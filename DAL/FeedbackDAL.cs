using SERVIGO.Helpers;
using System.Data;

namespace SERVIGO.DAL
{
    public static class FeedbackDAL
    {
        public static void Submit(string submittedBy, string reportType,
            string? targetUserID, string subject, string description)
        {
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO FeedbackReports (SubmittedBy, ReportType, TargetUserID, Subject, Description)
                  VALUES (@By, @Type, @Target, @Subj, @Desc)",
                DatabaseHelper.Param("@By",     submittedBy),
                DatabaseHelper.Param("@Type",   reportType),
                DatabaseHelper.Param("@Target", (object?)targetUserID ?? DBNull.Value),
                DatabaseHelper.Param("@Subj",   subject),
                DatabaseHelper.Param("@Desc",   description));
        }

        public static DataTable GetAll()
            => DatabaseHelper.ExecuteQuery(
                @"SELECT f.ReportID, f.ReportType, f.Subject, f.Description,
                         f.IsResolved, f.CreatedAt, f.ResolvedAt,
                         u.FullName AS SubmittedByName, f.SubmittedBy,
                         ISNULL(t.FullName, '') AS TargetName, ISNULL(f.TargetUserID, '') AS TargetUserID
                  FROM   FeedbackReports f
                  JOIN   Users u ON f.SubmittedBy = u.UserID
                  LEFT JOIN Users t ON f.TargetUserID = t.UserID
                  ORDER  BY f.IsResolved ASC, f.CreatedAt DESC");

        public static DataTable GetByUser(string userID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT ReportID, ReportType, Subject, Description,
                         IsResolved, CreatedAt, ResolvedAt
                  FROM   FeedbackReports
                  WHERE  SubmittedBy = @UID
                  ORDER  BY CreatedAt DESC",
                DatabaseHelper.Param("@UID", userID));

        public static void MarkResolved(int reportID)
        {
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE FeedbackReports SET IsResolved = 1, ResolvedAt = GETDATE() WHERE ReportID = @RID",
                DatabaseHelper.Param("@RID", reportID));
        }

        public static void MarkUnresolved(int reportID)
        {
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE FeedbackReports SET IsResolved = 0, ResolvedAt = NULL WHERE ReportID = @RID",
                DatabaseHelper.Param("@RID", reportID));
        }
    }
}
