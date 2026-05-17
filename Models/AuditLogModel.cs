namespace SERVIGO.Models
{
    public class AuditLogModel
    {
        public int      LogID       { get; set; }
        public string   TableName   { get; set; } = string.Empty;
        public string   Action      { get; set; } = string.Empty;
        public string   RecordID    { get; set; } = string.Empty;
        public string   PerformedBy { get; set; } = string.Empty;
        public string   Details     { get; set; } = string.Empty;
        public DateTime LoggedAt    { get; set; }

        public string LoggedAtDisplay => LoggedAt.ToString("MMM dd, yyyy  hh:mm:ss tt");
    }
}
