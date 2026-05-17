namespace SERVIGO.Models
{
    public class BookingModel
    {
        public int      BookingID    { get; set; }
        public string   CustomerID   { get; set; } = string.Empty;
        public string   CustomerName { get; set; } = string.Empty;
        public int      SlotID       { get; set; }
        public int      ServiceID    { get; set; }
        public string   ServiceName  { get; set; } = string.Empty;
        public int      ProviderID   { get; set; }
        public string   ProviderName { get; set; } = string.Empty;
        public int      StatusID     { get; set; }
        public string   StatusName   { get; set; } = string.Empty;
        public string?  Notes        { get; set; }
        public DateTime BookedAt     { get; set; }
        public DateTime UpdatedAt    { get; set; }
        public DateTime SlotDate     { get; set; }
        public TimeSpan StartTime    { get; set; }
        public TimeSpan EndTime      { get; set; }
        public decimal  Price        { get; set; }

        public string SlotDisplay    => $"{SlotDate:ddd, MMM dd yyyy}  {StartTime:hh\\:mm} – {EndTime:hh\\:mm}";
        public string BookedAtDisplay => BookedAt.ToString("MMM dd, yyyy  hh:mm tt");
    }
}
