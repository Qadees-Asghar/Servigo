using SERVIGO.Helpers;
using System.Data;

namespace SERVIGO.DAL
{
    public static class RatingDAL
    {
        public static bool HasRated(int bookingID)
        {
            var result = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Ratings WHERE BookingID = @BID",
                DatabaseHelper.Param("@BID", bookingID));
            return Convert.ToInt32(result) > 0;
        }

        public static void SubmitRating(int bookingID, int providerID, string customerID, int stars, string comment)
        {
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO Ratings (BookingID, ProviderID, CustomerID, Stars, Comment)
                  VALUES (@BID, @PID, @CID, @Stars, @Comment)",
                DatabaseHelper.Param("@BID",     bookingID),
                DatabaseHelper.Param("@PID",     providerID),
                DatabaseHelper.Param("@CID",     customerID),
                DatabaseHelper.Param("@Stars",   stars),
                DatabaseHelper.Param("@Comment", string.IsNullOrWhiteSpace(comment) ? (object)DBNull.Value : comment));
        }

        public static void UpdateRating(int bookingID, int stars, string comment)
        {
            DatabaseHelper.ExecuteNonQuery(
                @"UPDATE Ratings SET Stars = @Stars, Comment = @Comment
                  WHERE BookingID = @BID",
                DatabaseHelper.Param("@BID",     bookingID),
                DatabaseHelper.Param("@Stars",   stars),
                DatabaseHelper.Param("@Comment", string.IsNullOrWhiteSpace(comment) ? (object)DBNull.Value : comment));
        }

        public static (int Stars, string Comment) GetRating(int bookingID)
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT Stars, ISNULL(Comment,'') AS Comment FROM Ratings WHERE BookingID = @BID",
                DatabaseHelper.Param("@BID", bookingID));
            if (dt.Rows.Count == 0) return (0, string.Empty);
            return (Convert.ToInt32(dt.Rows[0]["Stars"]), dt.Rows[0]["Comment"].ToString()!);
        }

        // Completed bookings that haven't been rated yet
        public static DataTable GetUnreviewedBookings(string customerID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT b.BookingID, s.ServiceName, u.FullName AS ProviderName,
                         sp.ProviderID, ts.SlotDate, s.Price
                  FROM   Bookings b
                  JOIN   Services        s  ON b.ServiceID = s.ServiceID
                  JOIN   TimeSlots       ts ON b.SlotID    = ts.SlotID
                  JOIN   ServiceProviders sp ON ts.ProviderID = sp.ProviderID
                  JOIN   Users           u  ON sp.UserID   = u.UserID
                  WHERE  b.CustomerID = @UID
                    AND  b.StatusID = 3
                    AND  NOT EXISTS (SELECT 1 FROM Ratings r WHERE r.BookingID = b.BookingID)
                  ORDER  BY b.BookedAt DESC",
                DatabaseHelper.Param("@UID", customerID));

        // Completed bookings that HAVE been rated (with star info)
        public static DataTable GetReviewedBookings(string customerID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT b.BookingID, s.ServiceName, u.FullName AS ProviderName,
                         sp.ProviderID, ts.SlotDate, s.Price,
                         r.Stars, ISNULL(r.Comment, '') AS Comment
                  FROM   Bookings b
                  JOIN   Services        s  ON b.ServiceID = s.ServiceID
                  JOIN   TimeSlots       ts ON b.SlotID    = ts.SlotID
                  JOIN   ServiceProviders sp ON ts.ProviderID = sp.ProviderID
                  JOIN   Users           u  ON sp.UserID   = u.UserID
                  JOIN   Ratings         r  ON r.BookingID = b.BookingID
                  WHERE  b.CustomerID = @UID
                  ORDER  BY r.CreatedAt DESC",
                DatabaseHelper.Param("@UID", customerID));

        public static DataTable GetByProvider(int providerID)
            => DatabaseHelper.ExecuteQuery(
                @"SELECT r.Stars, r.Comment, r.CreatedAt,
                         ISNULL(u.FullName, 'Anonymous') AS CustomerName
                  FROM   Ratings r
                  LEFT JOIN Users u ON r.CustomerID = u.UserID
                  WHERE  r.ProviderID = @PID
                  ORDER  BY r.CreatedAt DESC",
                DatabaseHelper.Param("@PID", providerID));
    }
}
