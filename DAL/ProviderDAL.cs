using SERVIGO.Helpers;
using SERVIGO.Models;
using System.Data;

namespace SERVIGO.DAL
{
    public static class ProviderDAL
    {
        public static void CreateProvider(int categoryID, string userID, string description)
        {
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO ServiceProviders (UserID, CategoryID, Description, IsApproved)
                  VALUES (@UserID, @CatID, @Desc, 0)",
                DatabaseHelper.Param("@UserID", userID),
                DatabaseHelper.Param("@CatID",  categoryID),
                DatabaseHelper.Param("@Desc",   description));
        }

        public static void UpdateProvider(int providerID, int categoryID, string description)
        {
            DatabaseHelper.ExecuteNonQuery(
                @"UPDATE ServiceProviders
                  SET CategoryID = @CatID, Description = @Desc
                  WHERE ProviderID = @PID",
                DatabaseHelper.Param("@CatID", categoryID),
                DatabaseHelper.Param("@Desc",  description),
                DatabaseHelper.Param("@PID",   providerID));
        }

        public static void SetApproval(int providerID, bool approved)
        {
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE ServiceProviders SET IsApproved = @A WHERE ProviderID = @PID",
                DatabaseHelper.Param("@A",   approved),
                DatabaseHelper.Param("@PID", providerID));
        }

        public static void DeleteProvider(int providerID)
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM ServiceProviders WHERE ProviderID = @PID",
                DatabaseHelper.Param("@PID", providerID));
        }

        public static int? GetProviderIDByUserID(string userID)
        {
            var result = DatabaseHelper.ExecuteScalar(
                "SELECT ProviderID FROM ServiceProviders WHERE UserID = @UID",
                DatabaseHelper.Param("@UID", userID));
            return result == null || result == DBNull.Value
                ? null
                : Convert.ToInt32(result);
        }

        public static ServiceProviderUser? GetProviderByUserID(string userID)
        {
            var dt = DatabaseHelper.ExecuteQuery(
                @"SELECT u.UserID, u.FullName, u.Email, u.Phone, u.CNIC,
                         u.PasswordHash, u.IsActive, u.CreatedAt,
                         sp.ProviderID, sp.CategoryID, sc.CategoryName,
                         sp.Description, sp.IsApproved, sp.AverageRating
                  FROM   Users u
                  JOIN   ServiceProviders sp ON u.UserID     = sp.UserID
                  JOIN   ServiceCategories sc ON sp.CategoryID = sc.CategoryID
                  WHERE  u.UserID = @UID",
                DatabaseHelper.Param("@UID", userID));

            if (dt.Rows.Count == 0) return null;
            return MapToProvider(dt.Rows[0]);
        }

        public static DataTable GetAllApproved()
            => DatabaseHelper.ExecuteQuery(
                @"SELECT sp.ProviderID, u.UserID, u.FullName, u.Phone,
                         sc.CategoryName, sp.Description,
                         sp.AverageRating, sp.IsApproved,
                         dbo.fn_GetProviderCompletedCount(sp.ProviderID) AS CompletedBookings
                  FROM   ServiceProviders sp
                  JOIN   Users             u  ON sp.UserID     = u.UserID
                  JOIN   ServiceCategories sc ON sp.CategoryID = sc.CategoryID
                  WHERE  sp.IsApproved = 1 AND u.IsActive = 1
                  ORDER  BY u.FullName");

        public static DataTable GetAllApprovedByCategory(int categoryID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT sp.ProviderID, u.UserID, u.FullName, u.Phone,
                         sc.CategoryName, sp.Description, sp.AverageRating
                  FROM   ServiceProviders sp
                  JOIN   Users             u  ON sp.UserID     = u.UserID
                  JOIN   ServiceCategories sc ON sp.CategoryID = sc.CategoryID
                  WHERE  sp.IsApproved = 1 AND u.IsActive = 1
                    AND  sp.CategoryID = @CatID
                  ORDER  BY u.FullName",
                DatabaseHelper.Param("@CatID", categoryID));

        public static DataTable GetCategories()
            => DatabaseHelper.ExecuteQuery(
                "SELECT CategoryID, CategoryName FROM ServiceCategories ORDER BY CategoryName");

        private static ServiceProviderUser MapToProvider(DataRow row)
        {
            var p = new ServiceProviderUser
            {
                UserID        = row["UserID"].ToString()!,
                FullName      = row["FullName"].ToString()!,
                Email         = row["Email"].ToString()!,
                Phone         = row["Phone"].ToString()!.Trim(),
                CNIC          = row["CNIC"].ToString()!.Trim(),
                PasswordHash  = row["PasswordHash"].ToString()!,
                IsActive      = Convert.ToBoolean(row["IsActive"]),
                CreatedAt     = Convert.ToDateTime(row["CreatedAt"]),
                ProviderID    = Convert.ToInt32(row["ProviderID"]),
                CategoryID    = Convert.ToInt32(row["CategoryID"]),
                CategoryName  = row["CategoryName"].ToString()!,
                Description   = row["Description"].ToString()!,
                IsApproved    = Convert.ToBoolean(row["IsApproved"]),
                AverageRating = Convert.ToDecimal(row["AverageRating"])
            };
            return p;
        }
    }
}
