using SERVIGO.Helpers;
using SERVIGO.Models;
using System.Data;

namespace SERVIGO.DAL
{
    public static class NotificationDAL
    {
        public static int GetUnreadCount(string userID)
        {
            var result = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Notifications WHERE UserID = @UID AND IsRead = 0",
                DatabaseHelper.Param("@UID", userID));
            return Convert.ToInt32(result);
        }

        public static DataTable GetByUser(string userID, int top = 50)
            => DatabaseHelper.ExecuteQuery(
                $@"SELECT TOP {top} NotificationID, Message, IsRead, CreatedAt
                   FROM   Notifications
                   WHERE  UserID = @UID
                   ORDER  BY CreatedAt DESC",
                DatabaseHelper.Param("@UID", userID));

        public static void MarkRead(int notificationID)
            => DatabaseHelper.ExecuteNonQuery(
                "UPDATE Notifications SET IsRead = 1 WHERE NotificationID = @ID",
                DatabaseHelper.Param("@ID", notificationID));

        public static void MarkAllRead(string userID)
            => DatabaseHelper.ExecuteNonQuery(
                "UPDATE Notifications SET IsRead = 1 WHERE UserID = @UID AND IsRead = 0",
                DatabaseHelper.Param("@UID", userID));

        public static void Create(string userID, string message)
            => DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO Notifications (UserID, Message) VALUES (@UID, @Msg)",
                DatabaseHelper.Param("@UID", userID),
                DatabaseHelper.Param("@Msg", message));

        public static DataTable GetAuditLogs(int top = 200)
            => DatabaseHelper.ExecuteQuery(
                $@"SELECT TOP {top} LogID, TableName, Action, RecordID,
                          PerformedBy, Details, LoggedAt
                   FROM   AuditLogs
                   ORDER  BY LoggedAt DESC");
    }
}
