namespace SERVIGO.Models
{
    public class NotificationModel
    {
        public int      NotificationID { get; set; }
        public string   UserID         { get; set; } = string.Empty;
        public string   Message        { get; set; } = string.Empty;
        public bool     IsRead         { get; set; }
        public DateTime CreatedAt      { get; set; }

        public string TimeDisplay => CreatedAt.ToString("MMM dd  hh:mm tt");
    }
}
