using Microsoft.Data.SqlClient;
using SERVIGO.Helpers;
using SERVIGO.Models;
using System.Data;

namespace SERVIGO.DAL
{
    public static class BookingDAL
    {
        // ── Time Slots ────────────────────────────────────────────────────────

        public static int CreateSlot(int providerID, DateTime date, TimeSpan start, TimeSpan end)
        {
            var result = DatabaseHelper.ExecuteScalar(
                @"INSERT INTO TimeSlots (ProviderID, SlotDate, StartTime, EndTime, IsAvailable)
                  OUTPUT INSERTED.SlotID
                  VALUES (@PID, @Date, @Start, @End, 1)",
                DatabaseHelper.Param("@PID",   providerID),
                DatabaseHelper.Param("@Date",  date.Date),
                DatabaseHelper.Param("@Start", start),
                DatabaseHelper.Param("@End",   end));
            return Convert.ToInt32(result);
        }

        public static void DeleteSlot(int slotID)
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM TimeSlots WHERE SlotID = @SID AND IsAvailable = 1",
                DatabaseHelper.Param("@SID", slotID));
        }

        public static DataTable GetSlotsByProvider(int providerID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT SlotID, SlotDate, StartTime, EndTime, IsAvailable
                  FROM   TimeSlots
                  WHERE  ProviderID = @PID AND SlotDate >= CAST(GETDATE() AS DATE)
                  ORDER  BY SlotDate, StartTime",
                DatabaseHelper.Param("@PID", providerID));

        public static DataTable GetAvailableSlots(int providerID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT SlotID, SlotDate, StartTime, EndTime
                  FROM   TimeSlots
                  WHERE  ProviderID  = @PID
                    AND  IsAvailable = 1
                    AND  SlotDate   >= CAST(GETDATE() AS DATE)
                    AND  SlotDate   <= DATEADD(DAY, 7, CAST(GETDATE() AS DATE))
                  ORDER  BY SlotDate, StartTime",
                DatabaseHelper.Param("@PID", providerID));

        // ── Bookings ──────────────────────────────────────────────────────────

        public static int CreateBooking(string customerID, int slotID, int serviceID, string? notes)
        {
            var dt = DatabaseHelper.ExecuteStoredProcedure("sp_CreateBooking",
                DatabaseHelper.Param("@CustomerID", customerID),
                DatabaseHelper.Param("@SlotID",     slotID),
                DatabaseHelper.Param("@ServiceID",  serviceID),
                DatabaseHelper.Param("@Notes",      (object?)notes ?? DBNull.Value));

            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["NewBookingID"]);

            throw new InvalidOperationException("Booking creation returned no ID.");
        }

        public static void UpdateStatus(int bookingID, int newStatusID, string performedBy)
        {
            DatabaseHelper.ExecuteStoredProcedureNonQuery("sp_UpdateBookingStatus",
                DatabaseHelper.Param("@BookingID",   bookingID),
                DatabaseHelper.Param("@NewStatusID", newStatusID),
                DatabaseHelper.Param("@PerformedBy", performedBy));
        }

        public static DataTable GetCustomerBookings(string customerID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT b.BookingID, b.StatusID, bs.StatusName, b.BookedAt,
                         b.Notes, s.ServiceName, s.Price,
                         u.FullName AS ProviderName,
                         ts.SlotDate, ts.StartTime, ts.EndTime,
                         sp.ProviderID,
                         CASE WHEN EXISTS (
                             SELECT 1 FROM Ratings r WHERE r.BookingID = b.BookingID
                         ) THEN 1 ELSE 0 END AS HasRated
                  FROM   Bookings b
                  JOIN   BookingStatuses bs ON b.StatusID  = bs.StatusID
                  JOIN   Services        s  ON b.ServiceID = s.ServiceID
                  JOIN   TimeSlots       ts ON b.SlotID    = ts.SlotID
                  JOIN   ServiceProviders sp ON ts.ProviderID = sp.ProviderID
                  JOIN   Users           u  ON sp.UserID   = u.UserID
                  WHERE  b.CustomerID = @UID
                  ORDER  BY b.BookedAt DESC",
                DatabaseHelper.Param("@UID", customerID));

        public static DataTable GetProviderBookings(int providerID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT b.BookingID, b.StatusID, bs.StatusName, b.BookedAt,
                         b.Notes, s.ServiceName, s.Price,
                         uc.FullName AS CustomerName, uc.Phone AS CustomerPhone,
                         ts.SlotDate, ts.StartTime, ts.EndTime
                  FROM   Bookings b
                  JOIN   BookingStatuses bs ON b.StatusID  = bs.StatusID
                  JOIN   Services        s  ON b.ServiceID = s.ServiceID
                  JOIN   TimeSlots       ts ON b.SlotID    = ts.SlotID
                  JOIN   Users           uc ON b.CustomerID = uc.UserID
                  WHERE  ts.ProviderID = @PID
                  ORDER  BY ts.SlotDate DESC, ts.StartTime DESC",
                DatabaseHelper.Param("@PID", providerID));

        public static DataTable GetAllBookings()
            => DatabaseHelper.ExecuteQuery(
                @"SELECT b.BookingID, bs.StatusName, b.BookedAt,
                         s.ServiceName, s.Price,
                         uc.FullName AS CustomerName,
                         up.FullName AS ProviderName,
                         ts.SlotDate, ts.StartTime, ts.EndTime
                  FROM   Bookings b
                  JOIN   BookingStatuses  bs ON b.StatusID  = bs.StatusID
                  JOIN   Services         s  ON b.ServiceID = s.ServiceID
                  JOIN   TimeSlots        ts ON b.SlotID    = ts.SlotID
                  JOIN   Users            uc ON b.CustomerID = uc.UserID
                  JOIN   ServiceProviders sp ON ts.ProviderID = sp.ProviderID
                  JOIN   Users            up ON sp.UserID    = up.UserID
                  ORDER  BY b.BookedAt DESC");

        public static DataTable GetBookingSummary()
            => DatabaseHelper.ExecuteQuery("SELECT * FROM vw_BookingSummary");

        public static DataTable GetProviderStats()
            => DatabaseHelper.ExecuteQuery("SELECT * FROM vw_ProviderBookingStats ORDER BY TotalBookings DESC");

        public static DataTable GetDashboardStats()
            => DatabaseHelper.ExecuteStoredProcedure("sp_GetDashboardStats");
    }
}
