using Microsoft.Data.SqlClient;
using SERVIGO.Helpers;
using SERVIGO.Models;
using System.Data;

namespace SERVIGO.DAL
{
    public static class UserDAL
    {
        // ── ID generation ────────────────────────────────────────────────────

        public static string GenerateUserID()
        {
            var result = DatabaseHelper.ExecuteScalar(
                "SELECT dbo.fn_GenerateUserID()");
            return result?.ToString() ?? "SRV-00001";
        }

        // ── Existence checks ─────────────────────────────────────────────────

        public static bool AdminExists()
        {
            var result = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE RoleID = 1");
            return Convert.ToInt32(result) > 0;
        }

        public static bool EmailExists(string email, string? excludeUserID = null)
        {
            string sql = excludeUserID == null
                ? "SELECT COUNT(*) FROM Users WHERE Email = @Email"
                : "SELECT COUNT(*) FROM Users WHERE Email = @Email AND UserID <> @UID";

            var parms = excludeUserID == null
                ? new[] { DatabaseHelper.Param("@Email", email) }
                : new[] { DatabaseHelper.Param("@Email", email),
                          DatabaseHelper.Param("@UID", excludeUserID) };

            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(sql, parms)) > 0;
        }

        public static bool CNICExists(string cnic, string? excludeUserID = null)
        {
            string sql = excludeUserID == null
                ? "SELECT COUNT(*) FROM Users WHERE CNIC = @CNIC"
                : "SELECT COUNT(*) FROM Users WHERE CNIC = @CNIC AND UserID <> @UID";

            var parms = excludeUserID == null
                ? new[] { DatabaseHelper.Param("@CNIC", cnic) }
                : new[] { DatabaseHelper.Param("@CNIC", cnic),
                          DatabaseHelper.Param("@UID", excludeUserID) };

            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(sql, parms)) > 0;
        }

        public static bool PhoneExists(string phone, string? excludeUserID = null)
        {
            string sql = excludeUserID == null
                ? "SELECT COUNT(*) FROM Users WHERE Phone = @Phone"
                : "SELECT COUNT(*) FROM Users WHERE Phone = @Phone AND UserID <> @UID";

            var parms = excludeUserID == null
                ? new[] { DatabaseHelper.Param("@Phone", phone) }
                : new[] { DatabaseHelper.Param("@Phone", phone),
                          DatabaseHelper.Param("@UID", excludeUserID) };

            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(sql, parms)) > 0;
        }

        // ── CRUD ─────────────────────────────────────────────────────────────

        public static void CreateUser(User user)
        {
            string sql = @"
                INSERT INTO Users (UserID, FullName, Email, Phone, CNIC,
                                   PasswordHash, RoleID, IsActive, CreatedAt)
                VALUES (@UserID, @FullName, @Email, @Phone, @CNIC,
                        @PasswordHash, @RoleID, @IsActive, @CreatedAt)";

            DatabaseHelper.ExecuteNonQuery(sql,
                DatabaseHelper.Param("@UserID",       user.UserID),
                DatabaseHelper.Param("@FullName",     user.FullName),
                DatabaseHelper.Param("@Email",        user.Email),
                DatabaseHelper.Param("@Phone",        user.Phone),
                DatabaseHelper.Param("@CNIC",         user.CNIC),
                DatabaseHelper.Param("@PasswordHash", user.PasswordHash),
                DatabaseHelper.Param("@RoleID",       user.RoleID),
                DatabaseHelper.Param("@IsActive",     user.IsActive),
                DatabaseHelper.Param("@CreatedAt",    user.CreatedAt));
        }

        public static void UpdateUser(User user)
        {
            string sql = @"
                UPDATE Users SET
                    FullName = @FullName,
                    Email    = @Email,
                    Phone    = @Phone,
                    CNIC     = @CNIC,
                    IsActive = @IsActive
                WHERE UserID = @UserID";

            DatabaseHelper.ExecuteNonQuery(sql,
                DatabaseHelper.Param("@UserID",   user.UserID),
                DatabaseHelper.Param("@FullName", user.FullName),
                DatabaseHelper.Param("@Email",    user.Email),
                DatabaseHelper.Param("@Phone",    user.Phone),
                DatabaseHelper.Param("@CNIC",     user.CNIC),
                DatabaseHelper.Param("@IsActive", user.IsActive));
        }

        public static void UpdatePassword(string userID, string newHash)
        {
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Users SET PasswordHash = @Hash WHERE UserID = @UID",
                DatabaseHelper.Param("@Hash", newHash),
                DatabaseHelper.Param("@UID",  userID));
        }

        public static void DeleteUser(string userID)
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM Users WHERE UserID = @UID",
                DatabaseHelper.Param("@UID", userID));
        }

        public static void SetActiveStatus(string userID, bool isActive)
        {
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Users SET IsActive = @Active WHERE UserID = @UID",
                DatabaseHelper.Param("@Active", isActive),
                DatabaseHelper.Param("@UID",    userID));
        }

        // ── Authentication ────────────────────────────────────────────────────

        public static User? Login(string email, string userID, string plainPassword)
        {
            string sql = @"
                SELECT UserID, FullName, Email, Phone, CNIC,
                       PasswordHash, RoleID, IsActive, CreatedAt
                FROM   Users
                WHERE  Email  = @Email
                  AND  UserID = @UserID
                  AND  IsActive = 1";

            var dt = DatabaseHelper.ExecuteQuery(sql,
                DatabaseHelper.Param("@Email",  email),
                DatabaseHelper.Param("@UserID", userID));

            if (dt.Rows.Count == 0) return null;

            var row  = dt.Rows[0];
            string hash = row["PasswordHash"].ToString()!;

            if (!PasswordHelper.Verify(plainPassword, hash)) return null;

            return MapToUser(row);
        }

        // ── Queries ───────────────────────────────────────────────────────────

        public static User? GetByID(string userID)
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM Users WHERE UserID = @UID",
                DatabaseHelper.Param("@UID", userID));

            return dt.Rows.Count > 0 ? MapToUser(dt.Rows[0]) : null;
        }

        public static DataTable GetAllCustomers()
            => DatabaseHelper.ExecuteQuery(
                @"SELECT u.UserID, u.FullName, u.Email, u.Phone, u.CNIC,
                         u.IsActive, u.CreatedAt,
                         dbo.fn_GetCustomerBookingCount(u.UserID) AS TotalBookings
                  FROM   Users u
                  WHERE  u.RoleID = 2
                  ORDER  BY u.CreatedAt DESC");

        public static DataTable GetAllProviders()
            => DatabaseHelper.ExecuteQuery(
                @"SELECT u.UserID, u.FullName, u.Email, u.Phone, u.CNIC,
                         u.IsActive, u.CreatedAt,
                         sc.CategoryName, sp.IsApproved, sp.AverageRating,
                         sp.ProviderID
                  FROM   Users u
                  JOIN   ServiceProviders sp ON u.UserID = sp.UserID
                  JOIN   ServiceCategories sc ON sp.CategoryID = sc.CategoryID
                  WHERE  u.RoleID = 3
                  ORDER  BY u.CreatedAt DESC");

        // ── Mapper ────────────────────────────────────────────────────────────

        private static User MapToUser(DataRow row)
        {
            int roleID = Convert.ToInt32(row["RoleID"]);
            User user = roleID switch
            {
                1 => new AdminUser(),
                2 => new CustomerUser(),
                3 => new ServiceProviderUser(),
                _ => throw new InvalidOperationException($"Unknown RoleID: {roleID}")
            };

            user.UserID       = row["UserID"].ToString()!;
            user.FullName     = row["FullName"].ToString()!;
            user.Email        = row["Email"].ToString()!;
            user.Phone        = row["Phone"].ToString()!.Trim();
            user.CNIC         = row["CNIC"].ToString()!.Trim();
            user.PasswordHash = row["PasswordHash"].ToString()!;
            user.RoleID       = roleID;
            user.IsActive     = Convert.ToBoolean(row["IsActive"]);
            user.CreatedAt    = Convert.ToDateTime(row["CreatedAt"]);
            return user;
        }
    }
}
