using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BusBookingProject.Security;

namespace BusBookingProject.Services
{
    public class ScheduleInfo
    {
        public int ScheduleId, BusId, SeatRow, SeatColumn, TotalSeat;
        public string BusName, BusNo, BusType, EstimatedTime, DepartureTime, ArrivalTime, TravelDate, Origin, Destination;
        public decimal Fare;
        public bool IsBookable;
    }

    public class BoardingPoint
    {
        public int Id;
        public string Place, Time;
    }

    public class PassengerInfo
    {
        // must be PROPERTIES (not fields): ASP.NET data binding (Eval) can only read properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string SeatNo { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public int? Age { get; set; }
        public decimal Fare { get; set; }
        public string FullName { get { return (FirstName + " " + LastName).Trim(); } }
    }

    public class BookingDetail
    {
        public string Pnr, Status, ContactEmail, ContactMobile, BusName, BusNo, BusType, Origin, Destination, TravelDate,
                      DepartureTime, ArrivalTime, BoardingPlace, BoardingTime, PaymentId, OrderId, PaymentMethod, PaymentStatus;
        public decimal TotalAmount;
        public int TotalTickets, UserId, ScheduleId;
        public DateTime? CreatedAt, ConfirmedAt, HoldExpiresAt, PaidAt;
        public List<PassengerInfo> Passengers = new List<PassengerInfo>();
    }

    /// <summary>State carried between seat selection, passenger details and payment (server-side only).</summary>
    [Serializable]
    public class CheckoutState
    {
        public int ScheduleId, BoardingId;
        public List<int> Seats = new List<int>();
        public string Pnr, OrderId;
        public decimal Amount;
        public DateTime HoldExpiresAt;
    }

    public static class BookingService
    {
        public static int MaxSeats { get { return Math.Max(1, Math.Min(10, Config.GetInt("Booking:MaxSeatsPerBooking", 3))); } }
        public static int HoldMinutes { get { return Math.Max(3, Math.Min(30, Config.GetInt("Booking:SeatHoldMinutes", 10))); } }

        private const string PnrAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";   // no 0/O/1/I

        public static string NewPnr() { return Crypto.RandomString(8, PnrAlphabet); }

        private static string S(object o) { return o == null || o == DBNull.Value ? string.Empty : Convert.ToString(o); }
        private static DateTime? D(object o) { return o == null || o == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(o); }

        public static ScheduleInfo GetSchedule(int scheduleId)
        {
            DataSet ds = Db.Proc("dbo.ispGetScheduleForBooking", Db.P("@ScheduleId", scheduleId));
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return null;
            DataRow r = ds.Tables[0].Rows[0];
            return new ScheduleInfo
            {
                ScheduleId = Convert.ToInt32(r["ScheduleId"]),
                BusId = Convert.ToInt32(r["BusId"]),
                SeatRow = Convert.ToInt32(r["SeatRow"]),
                SeatColumn = Convert.ToInt32(r["SeatColumn"]),
                TotalSeat = Convert.ToInt32(r["TotalSeat"]),
                BusName = S(r["BusName"]),
                BusNo = S(r["BusNo"]),
                BusType = S(r["BustType"]),
                EstimatedTime = S(r["EstimatedTime"]),
                DepartureTime = S(r["DepartureTime"]),
                ArrivalTime = S(r["ArivalTime"]),
                TravelDate = S(r["TravelDate"]),
                Origin = S(r["Origin"]),
                Destination = S(r["Destination"]),
                Fare = Convert.ToDecimal(r["Fare"]),
                IsBookable = Convert.ToInt32(r["IsBookable"]) == 1
            };
        }

        public static List<BoardingPoint> GetBoardingPoints(int busId)
        {
            var list = new List<BoardingPoint>();
            DataTable dt = Db.Query("SELECT StandId, PlaceName, PlaceTime FROM dbo.PickUpStand WHERE BusID = @b ORDER BY StandId", Db.P("@b", busId));
            foreach (DataRow r in dt.Rows)
                list.Add(new BoardingPoint { Id = Convert.ToInt32(r["StandId"]), Place = S(r["PlaceName"]), Time = S(r["PlaceTime"]) });
            return list;
        }

        /// <summary>Seat numbers by state: Booked, Held (by someone else), Mine.</summary>
        public static void GetSeatStatus(int scheduleId, int userId, List<int> booked, List<int> held, List<int> mine)
        {
            DataSet ds = Db.Proc("dbo.ispGetSeatStatus", Db.P("@ScheduleId", scheduleId), Db.P("@UserId", userId));
            if (ds.Tables.Count == 0) return;
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                int seat = Convert.ToInt32(r["SeatNo"]);
                switch (S(r["State"]))
                {
                    case "Booked": booked.Add(seat); break;
                    case "Held": held.Add(seat); break;
                    default: mine.Add(seat); break;
                }
            }
        }

        /// <summary>Returns the number of seats held, or a negative code (-1 taken, -2 invalid seat, -3 unknown trip, -4 none).</summary>
        public static int HoldSeats(int scheduleId, int userId, IList<int> seats, out int conflictSeat)
        {
            using (SqlConnection c = Db.Open())
            using (var cmd = new SqlCommand("dbo.ispHoldSeats", c) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Seats", string.Join(",", seats));
                cmd.Parameters.AddWithValue("@HoldMinutes", HoldMinutes);
                var result = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var conflict = new SqlParameter("@ConflictSeat", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(result);
                cmd.Parameters.Add(conflict);
                cmd.ExecuteNonQuery();
                conflictSeat = conflict.Value == DBNull.Value ? 0 : Convert.ToInt32(conflict.Value);
                return Convert.ToInt32(result.Value);
            }
        }

        public static void ReleaseHolds(int scheduleId, int userId)
        {
            Db.NonQuery("EXEC dbo.ispReleaseHolds @s, @u", Db.P("@s", scheduleId), Db.P("@u", userId));
        }

        /// <summary>Creates the pending booking. The amount comes back from the database (schedule fare x seats).</summary>
        public static int CreatePending(int userId, int scheduleId, int boardingId, string pnr, string email, string mobile,
                                        DataTable passengers, out decimal totalAmount)
        {
            using (SqlConnection c = Db.Open())
            using (var cmd = new SqlCommand("dbo.ispCreatePendingBooking", c) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                cmd.Parameters.AddWithValue("@BoardingId", boardingId);
                cmd.Parameters.AddWithValue("@PNRNo", pnr);
                cmd.Parameters.AddWithValue("@ContactEmail", email);
                cmd.Parameters.AddWithValue("@ContactMobile", (object)mobile ?? DBNull.Value);
                var tvp = cmd.Parameters.AddWithValue("@Passengers", passengers);
                tvp.SqlDbType = SqlDbType.Structured;
                tvp.TypeName = "dbo.PassengerList";
                var total = new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };
                var result = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(total);
                cmd.Parameters.Add(result);
                cmd.ExecuteNonQuery();
                totalAmount = total.Value == DBNull.Value ? 0m : Convert.ToDecimal(total.Value);
                return Convert.ToInt32(result.Value);
            }
        }

        public static void RecordPaymentOrder(string pnr, int userId, decimal amount, string orderId)
        {
            Db.NonQuery(@"INSERT INTO dbo.Payments(PNRNo, UserId, Amount, Currency, Gateway, GatewayOrderId, Status)
                          VALUES(@p, @u, @a, 'INR', 'Razorpay', @o, 'Created')",
                Db.P("@p", pnr), Db.P("@u", userId), Db.P("@a", amount), Db.P("@o", orderId));
        }

        public static string FindOrderForPnr(string pnr, int userId)
        {
            object o = Db.Scalar("SELECT TOP 1 GatewayOrderId FROM dbo.Payments WHERE PNRNo = @p AND UserId = @u AND Status = 'Created' ORDER BY PaymentId DESC",
                Db.P("@p", pnr), Db.P("@u", userId));
            return o == null ? null : Convert.ToString(o);
        }

        /// <summary>1 confirmed, 2 already confirmed, -1 not found, -2 amount mismatch, -3 seat lost.</summary>
        public static int Confirm(string pnr, int userId, string orderId, string paymentId, string method, decimal amountPaid)
        {
            using (SqlConnection c = Db.Open())
            using (var cmd = new SqlCommand("dbo.ispConfirmBooking", c) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@PNRNo", pnr);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@GatewayOrderId", orderId);
                cmd.Parameters.AddWithValue("@GatewayPaymentId", paymentId);
                cmd.Parameters.AddWithValue("@Method", (object)method ?? "unknown");
                var amt = cmd.Parameters.AddWithValue("@AmountPaid", amountPaid);
                amt.SqlDbType = SqlDbType.Decimal; amt.Precision = 18; amt.Scale = 2;
                var result = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(result);
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(result.Value);
            }
        }

        public static void MarkPayment(string orderId, string status, string paymentId, string reason, string refundId)
        {
            Db.NonQuery(@"UPDATE dbo.Payments SET Status = @s, GatewayPaymentId = ISNULL(@pid, GatewayPaymentId), FailureReason = @r, RefundId = ISNULL(@rid, RefundId)
                          WHERE GatewayOrderId = @o AND Status <> 'Paid'",
                Db.P("@s", status), Db.P("@pid", paymentId), Db.P("@r", reason == null ? null : (reason.Length > 200 ? reason.Substring(0, 200) : reason)),
                Db.P("@rid", refundId), Db.P("@o", orderId));
        }

        public static void CancelPending(string pnr, int userId)
        {
            Db.NonQuery("EXEC dbo.ispCancelPendingBooking @p, @u", Db.P("@p", pnr), Db.P("@u", userId));
        }

        /// <param name="userId">Pass the signed-in user's id to enforce ownership; null for trusted server-side callers (webhook).</param>
        public static BookingDetail GetBooking(string pnr, int? userId)
        {
            if (!Validator.IsPnr(pnr)) return null;
            DataSet ds = Db.Proc("dbo.ispGetBookingByPNR", Db.P("@PNRNo", pnr), Db.P("@UserId", userId));
            if (ds.Tables.Count < 2 || ds.Tables[0].Rows.Count == 0) return null;
            DataRow r = ds.Tables[0].Rows[0];
            var b = new BookingDetail
            {
                Pnr = S(r["PNRNo"]),
                Status = S(r["Status"]),
                ContactEmail = S(r["ContactEmail"]),
                ContactMobile = S(r["ContactMobile"]),
                BusName = S(r["BusName"]),
                BusNo = S(r["BusNo"]),
                BusType = S(r["BustType"]),
                Origin = S(r["Origin"]),
                Destination = S(r["Destination"]),
                TravelDate = S(r["TravelDate"]),
                DepartureTime = S(r["DepartureTime"]),
                ArrivalTime = S(r["ArivalTime"]),
                BoardingPlace = S(r["BoardingPlace"]),
                BoardingTime = S(r["BoardingTime"]),
                PaymentId = S(r["GatewayPaymentId"]),
                OrderId = S(r["GatewayOrderId"]),
                PaymentMethod = S(r["PaymentMethod"]),
                PaymentStatus = S(r["PaymentStatus"]),
                TotalAmount = Convert.ToDecimal(r["TotalAmount"]),
                TotalTickets = Convert.ToInt32(r["TotalTickets"]),
                UserId = Convert.ToInt32(r["UserId"]),
                ScheduleId = r["ScheduleId"] == DBNull.Value ? 0 : Convert.ToInt32(r["ScheduleId"]),
                CreatedAt = D(r["CreatedAt"]),
                ConfirmedAt = D(r["ConfirmedAt"]),
                HoldExpiresAt = D(r["HoldExpiresAt"]),
                PaidAt = D(r["PaidAt"])
            };
            foreach (DataRow p in ds.Tables[1].Rows)
            {
                b.Passengers.Add(new PassengerInfo
                {
                    FirstName = S(p["Fname"]),
                    LastName = S(p["Lname"]),
                    Gender = S(p["Gender"]),
                    SeatNo = S(p["SeatNo"]),
                    Age = p["Age"] == DBNull.Value ? (int?)null : Convert.ToInt32(p["Age"]),
                    Fare = p["Fare"] == DBNull.Value ? 0m : Convert.ToDecimal(p["Fare"]),
                    Email = S(p["Email"]),
                    Contact = S(p["Contact"])
                });
            }
            return b;
        }

        public static DataTable ListUserBookings(int userId)
        {
            DataSet ds = Db.Proc("dbo.ispGetPNRDetails", Db.P("@UserID", userId));
            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        public static DataTable NewPassengerTable()
        {
            var t = new DataTable();
            t.Columns.Add("SeatNo", typeof(int));
            t.Columns.Add("Fname", typeof(string));
            t.Columns.Add("Lname", typeof(string));
            t.Columns.Add("Age", typeof(byte));
            t.Columns.Add("Gender", typeof(string));
            t.Columns.Add("Email", typeof(string));
            t.Columns.Add("Contact", typeof(string));
            t.Columns.Add("City", typeof(string));
            return t;
        }
    }
}