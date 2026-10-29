using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class PassengerDetailsInfo : BasePage
    {
        protected override bool RequiresLogin { get { return true; } }
        protected int HoldSecondsLeft;

        private CheckoutState state;
        private ScheduleInfo schedule;

        protected void Page_Load(object sender, EventArgs e)
        {
            state = Session["Checkout"] as CheckoutState;
            if (state == null || state.Seats.Count == 0)
            {
                Flash("Please choose your seats first.", "info");
                Response.Redirect("Home.aspx", true);
                return;
            }
            schedule = BookingService.GetSchedule(state.ScheduleId);
            if (schedule == null || !schedule.IsBookable)
            {
                Flash("Booking for this departure has closed.", "warning");
                Response.Redirect("Home.aspx", true);
                return;
            }

            if (IsPostBack) { HoldSecondsLeft = SecondsLeft(); return; }

            // Reserve the seats atomically. If someone else got there first, go back to the seat map.
            int conflict;
            int held = BookingService.HoldSeats(state.ScheduleId, AuthService.UserId, state.Seats, out conflict);
            if (held <= 0)
            {
                Flash(held == -1 ? "Seat " + conflict + " was just booked by someone else. Please choose again."
                                 : "We could not reserve those seats. Please choose again.", "warning");
                Response.Redirect("SeatDetails.aspx?sid=" + state.ScheduleId, true);
                return;
            }
            state.HoldExpiresAt = DateTime.Now.AddMinutes(BookingService.HoldMinutes);
            HoldSecondsLeft = SecondsLeft();

            BoardingPoint bp = BookingService.GetBoardingPoints(schedule.BusId).FirstOrDefault(b => b.Id == state.BoardingId);
            litRoute.Text = Server.HtmlEncode(schedule.Origin) + " <i class=\"fa-solid fa-arrow-right mx-1\"></i> " + Server.HtmlEncode(schedule.Destination);
            litTrip.Text = Server.HtmlEncode(schedule.BusName) + " &middot; " + Server.HtmlEncode(schedule.TravelDate) + " &middot; " + Server.HtmlEncode(schedule.DepartureTime);
            litSeats.Text = Server.HtmlEncode(string.Join(", ", state.Seats));
            litBoarding.Text = bp == null ? "-" : Server.HtmlEncode(bp.Place + " (" + bp.Time + ")");
            litFare.Text = schedule.Fare.ToString("N0");
            litTotal.Text = (schedule.Fare * state.Seats.Count).ToString("N0");

            rptPassengers.DataSource = state.Seats.Select(s => new { Seat = s }).ToList();
            rptPassengers.DataBind();

            UserRecord u = UserRepository.FindById(AuthService.UserId);
            if (u != null)
            {
                txtEmail.Text = u.Email ?? "";
                txtMobile.Text = Validator.NormalizeMobile(u.Mobile) ?? "";
                if (rptPassengers.Items.Count > 0)
                {
                    ((TextBox)rptPassengers.Items[0].FindControl("txtFirst")).Text = u.FirstName;
                    ((TextBox)rptPassengers.Items[0].FindControl("txtLast")).Text = u.LastName;
                }
            }
        }

        private int SecondsLeft()
        {
            return Math.Max(0, (int)(state.HoldExpiresAt - DateTime.Now).TotalSeconds);
        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            if (state == null || schedule == null) return;
            if (!RazorpayService.IsConfigured)
            {
                Toast("Online payment is not available right now. Please try again later.", "danger");
                return;
            }

            string email = Validator.NormalizeEmail(txtEmail.Text);
            string mobile = Validator.NormalizeMobile(txtMobile.Text);
            if (!Validator.IsEmail(email)) { Toast("Please enter a valid e-mail address.", "danger"); return; }
            if (mobile == null) { Toast("Please enter a valid 10-digit Indian mobile number.", "danger"); return; }

            DataTable pax = BookingService.NewPassengerTable();
            foreach (RepeaterItem item in rptPassengers.Items)
            {
                int seat;
                string first = Validator.Clean(((TextBox)item.FindControl("txtFirst")).Text, 50);
                string last = Validator.Clean(((TextBox)item.FindControl("txtLast")).Text, 50);
                string gender = ((DropDownList)item.FindControl("ddlGender")).SelectedValue;
                int age;
                if (!int.TryParse(((HiddenField)item.FindControl("hdnSeat")).Value, NumberStyles.None, CultureInfo.InvariantCulture, out seat) || !state.Seats.Contains(seat))
                { Toast("The seat selection changed. Please start again.", "danger"); return; }
                if (!Validator.IsName(first) || !Validator.IsName(last)) { Toast("Please enter each passenger's first and last name (letters only).", "danger"); return; }
                if (!int.TryParse(((TextBox)item.FindControl("txtAge")).Text, NumberStyles.None, CultureInfo.InvariantCulture, out age) || age < 1 || age > 120)
                { Toast("Please enter a valid age for every passenger.", "danger"); return; }
                if (gender != "M" && gender != "F" && gender != "O") { Toast("Please choose a gender for every passenger.", "danger"); return; }

                DataRow r = pax.NewRow();
                r["SeatNo"] = seat; r["Fname"] = first; r["Lname"] = last; r["Age"] = (byte)age; r["Gender"] = gender;
                r["Email"] = email; r["Contact"] = mobile; r["City"] = DBNull.Value;
                pax.Rows.Add(r);
            }
            if (pax.Rows.Count != state.Seats.Count) { Toast("Please fill in every passenger.", "danger"); return; }

            // (Re)create the hold in case the page sat open and it lapsed; the booking itself is verified in the database.
            int userId = AuthService.UserId, conflict;
            int held = BookingService.HoldSeats(state.ScheduleId, userId, state.Seats, out conflict);
            if (held <= 0)
            {
                Flash("Your seat hold expired and a seat was taken. Please choose again.", "warning");
                Response.Redirect("SeatDetails.aspx?sid=" + state.ScheduleId, true);
                return;
            }

            string pnr = BookingService.NewPnr();
            decimal total;
            int result = BookingService.CreatePending(userId, state.ScheduleId, state.BoardingId, pnr, email, mobile, pax, out total);
            if (result <= 0 || total <= 0)
            {
                Toast("We could not create the booking. Please try again.", "danger");
                return;
            }

            string error;
            string orderId = RazorpayService.CreateOrder((long)Math.Round(total * 100m), pnr, out error);
            if (orderId == null)
            {
                BookingService.CancelPending(pnr, userId);
                Toast("The payment gateway is unavailable right now. Please try again in a moment.", "danger");
                return;
            }
            BookingService.RecordPaymentOrder(pnr, userId, total, orderId);
            Db.Audit(userId, "booking_pending", pnr, ClientIp, UserAgent);

            state.Pnr = pnr; state.OrderId = orderId; state.Amount = total;
            Response.Redirect("Payment.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (state != null)
            {
                BookingService.ReleaseHolds(state.ScheduleId, AuthService.UserId);
                if (!string.IsNullOrEmpty(state.Pnr)) BookingService.CancelPending(state.Pnr, AuthService.UserId);
            }
            Session.Remove("Checkout");
            Flash("Your seats were released.", "info");
            Response.Redirect("Home.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
