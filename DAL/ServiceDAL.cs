using SERVIGO.Helpers;
using SERVIGO.Models;
using System.Data;

namespace SERVIGO.DAL
{
    public static class ServiceDAL
    {
        public static int CreateService(ServiceModel s)
        {
            var result = DatabaseHelper.ExecuteScalar(
                @"INSERT INTO Services (ProviderID, ServiceName, Description, Price, DurationMinutes, IsActive)
                  OUTPUT INSERTED.ServiceID
                  VALUES (@PID, @Name, @Desc, @Price, @Dur, 1)",
                DatabaseHelper.Param("@PID",   s.ProviderID),
                DatabaseHelper.Param("@Name",  s.ServiceName),
                DatabaseHelper.Param("@Desc",  s.Description),
                DatabaseHelper.Param("@Price", s.Price),
                DatabaseHelper.Param("@Dur",   s.DurationMinutes));
            return Convert.ToInt32(result);
        }

        public static void UpdateService(ServiceModel s)
        {
            DatabaseHelper.ExecuteNonQuery(
                @"UPDATE Services SET
                    ServiceName = @Name, Description = @Desc,
                    Price = @Price, DurationMinutes = @Dur, IsActive = @Active
                  WHERE ServiceID = @SID",
                DatabaseHelper.Param("@SID",    s.ServiceID),
                DatabaseHelper.Param("@Name",   s.ServiceName),
                DatabaseHelper.Param("@Desc",   s.Description),
                DatabaseHelper.Param("@Price",  s.Price),
                DatabaseHelper.Param("@Dur",    s.DurationMinutes),
                DatabaseHelper.Param("@Active", s.IsActive));
        }

        public static void DeleteService(int serviceID)
        {
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Services SET IsActive = 0 WHERE ServiceID = @SID",
                DatabaseHelper.Param("@SID", serviceID));
        }

        public static DataTable GetByProvider(int providerID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT ServiceID, ServiceName, Description, Price, IsActive
                  FROM   Services
                  WHERE  ProviderID = @PID
                  ORDER  BY ServiceName",
                DatabaseHelper.Param("@PID", providerID));

        public static DataTable GetActiveByProvider(int providerID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT ServiceID, ServiceName, Description, Price
                  FROM   Services
                  WHERE  ProviderID = @PID AND IsActive = 1
                  ORDER  BY ServiceName",
                DatabaseHelper.Param("@PID", providerID));

        public static DataTable SearchServices(string keyword, int? categoryID)
        {
            string sql = @"
                SELECT s.ServiceID, s.ServiceName, s.Description, s.Price,
                       u.FullName AS ProviderName,
                       sc.CategoryName, sp.ProviderID, sp.AverageRating
                FROM   Services s
                JOIN   ServiceProviders sp ON s.ProviderID  = sp.ProviderID
                JOIN   Users             u  ON sp.UserID     = u.UserID
                JOIN   ServiceCategories sc ON sp.CategoryID = sc.CategoryID
                WHERE  s.IsActive = 1 AND u.IsActive = 1
                  AND  (@Keyword = '' OR s.ServiceName LIKE '%' + @Keyword + '%'
                                     OR u.FullName    LIKE '%' + @Keyword + '%')
                  AND  (@CatID IS NULL OR sp.CategoryID = @CatID)
                ORDER  BY sp.AverageRating DESC, u.FullName";

            return DatabaseHelper.ExecuteQuery(sql,
                DatabaseHelper.Param("@Keyword", keyword ?? string.Empty),
                DatabaseHelper.Param("@CatID",   (object?)categoryID ?? DBNull.Value));
        }

        public static ServiceModel? GetByID(int serviceID)
        {
            var dt = DatabaseHelper.ExecuteQuery(
                @"SELECT s.*, u.FullName AS ProviderName, sc.CategoryName
                  FROM   Services s
                  JOIN   ServiceProviders sp ON s.ProviderID  = sp.ProviderID
                  JOIN   Users             u  ON sp.UserID     = u.UserID
                  JOIN   ServiceCategories sc ON sp.CategoryID = sc.CategoryID
                  WHERE  s.ServiceID = @SID",
                DatabaseHelper.Param("@SID", serviceID));

            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new ServiceModel
            {
                ServiceID       = Convert.ToInt32(r["ServiceID"]),
                ProviderID      = Convert.ToInt32(r["ProviderID"]),
                ProviderName    = r["ProviderName"].ToString()!,
                CategoryName    = r["CategoryName"].ToString()!,
                ServiceName     = r["ServiceName"].ToString()!,
                Description     = r["Description"].ToString()!,
                Price           = Convert.ToDecimal(r["Price"]),
                DurationMinutes = Convert.ToInt32(r["DurationMinutes"]),
                IsActive        = Convert.ToBoolean(r["IsActive"])
            };
        }
    }
}
