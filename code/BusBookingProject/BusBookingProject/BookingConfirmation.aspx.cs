using System;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class BookingConfirmation : BasePage
    {
        protected override bool RequiresLogin { get { return true; } }
        private BookingDetail booking;

        protected void Page_Load(object sender, EventArgs e)
        {
            booking = BookingService.GetBooking(Request.QueryString["pnr"], AuthService.UserId);   // ownership enforced in SQL
            if (booking == null || booking.Status != "Confirmed")
            {
                Flash("That booking could not be found.", "warning");
                Response.Redirect("BookingReport.aspx", true);
                return;
            }
            lnkTicket.HRef = "Download.ashx?pnr=" + booking.Pnr + "&type=ticket";
            lnkReceipt.HRef = "Download.ashx?pnr=" + booking.Pnr + "&type=receipt";
            if (IsPostBack) return;

            litEmail.Text = Server.HtmlEncode(booking.ContactEmail);
            litPnr.Text = Server.HtmlEncode(booking.Pnr);
            litAmount.Text = booking.TotalAmount.ToString("N2");
            litRoute.Text = Server.HtmlEncode(booking.Origin) + " <i class=\"fa-solid fa-arrow-right mx-1\"></i> " + Server.HtmlEncode(booking.Destination);
            litTrip.Text = Server.HtmlEncode(booking.BusName) + " (" + Server.HtmlEncode(booking.BusNo) + ") &middot; " + Server.HtmlEncode(booking.TravelDate)
                + " &middot; departs " + Server.HtmlEncode(booking.DepartureTime);
            litBoarding.Text = Server.HtmlEncode(booking.BoardingPlace) + " at " + Server.HtmlEncode(booking.BoardingTime);
            rptPax.DataSource = booking.Passengers;
            rptPax.DataBind();
        }

        protected void btnResend_Click(object sender, EventArgs e)
        {
            if (!Throttle.Allow("resend:" + booking.Pnr, 3, TimeSpan.FromHours(1)))
            {
                Toast("You have asked for this ticket several times. Please check your inbox and spam folder.", "warning");
                return;
            }
            EmailService.SendBookingConfirmed(booking.Pnr, AuthService.UserId);
            Toast("We have sent the ticket and receipt again.", "success");
        }
    }
}
