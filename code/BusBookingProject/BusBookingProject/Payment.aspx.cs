using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class Payment : BasePage
    {
        protected override bool RequiresLogin { get { return true; } }

        private static readonly Regex PayIdRx = new Regex(@"^pay_[A-Za-z0-9]{8,30}$", RegexOptions.Compiled);
        private CheckoutState state;
        private BookingDetail booking;

        protected void Page_Load(object sender, EventArgs e)
        {
            state = Session["Checkout"] as CheckoutState;
            if (state == null || string.IsNullOrEmpty(state.Pnr) || string.IsNullOrEmpty(state.OrderId))
            {
                Flash("There is no payment in progress.", "info");
                Response.Redirect("BookingReport.aspx", true);
                return;
            }
            booking = BookingService.GetBooking(state.Pnr, AuthService.UserId);
            if (booking == null) { Response.Redirect("BookingReport.aspx", true); return; }
            if (booking.Status == "Confirmed") { Response.Redirect("BookingConfirmation.aspx?pnr=" + booking.Pnr, true); return; }
            if (!RazorpayService.IsConfigured) { Flash("Online payment is not available right now.", "danger"); Response.Redirect("Home.aspx", true); return; }

            int left = Math.Max(0, (int)(state.HoldExpiresAt - DateTime.Now).TotalSeconds);
            pnlTimer.Attributes["data-hold-until"] = left.ToString();
            bool expired = left <= 0;
            pnlExpired.Visible = expired;
            payBox.Visible = !expired;

            // everything the checkout window needs is rendered as encoded data attributes (no inline script, no injection)
            payBox.Attributes["data-key"] = RazorpayService.KeyId;
            payBox.Attributes["data-order"] = state.OrderId;
            payBox.Attributes["data-amount"] = ((long)Math.Round(booking.TotalAmount * 100m)).ToString();
            payBox.Attributes["data-pnr"] = booking.Pnr;
            payBox.Attributes["data-name"] = AuthService.DisplayName;
            payBox.Attributes["data-email"] = booking.ContactEmail;
            payBox.Attributes["data-contact"] = string.IsNullOrEmpty(booking.ContactMobile) ? "" : "+91" + booking.ContactMobile;
            payBox.Attributes["data-f-payment"] = hdnPayment.ClientID;
            payBox.Attributes["data-f-order"] = hdnOrder.ClientID;
            payBox.Attributes["data-f-signature"] = hdnSignature.ClientID;
            payBox.Attributes["data-verify"] = btnVerify.ClientID;

            if (IsPostBack) return;

            litRoute.Text = Server.HtmlEncode(booking.Origin) + " <i class=\"fa-solid fa-arrow-right mx-1\"></i> " + Server.HtmlEncode(booking.Destination);
            litTrip.Text = Server.HtmlEncode(booking.BusName) + " &middot; " + Server.HtmlEncode(booking.TravelDate) + " &middot; departs " + Server.HtmlEncode(booking.DepartureTime)
                + "<br />Boarding: " + Server.HtmlEncode(booking.BoardingPlace) + " (" + Server.HtmlEncode(booking.BoardingTime) + ")";
            litTotal.Text = booking.TotalAmount.ToString("N2");
            rptPax.DataSource = booking.Passengers;
            rptPax.DataBind();
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            if (state == null || booking == null) return;
            int userId = AuthService.UserId;
            string ip = ClientIp;

            string orderId = (hdnOrder.Value ?? "").Trim();
            string paymentId = (hdnPayment.Value ?? "").Trim();
            string signature = (hdnSignature.Value ?? "").Trim();

            // 1. the callback must belong to OUR order and carry a valid signature
            if (!Crypto.FixedTimeEquals(orderId, state.OrderId) || !PayIdRx.IsMatch(paymentId)
                || !RazorpayService.VerifyCheckoutSignature(state.OrderId, paymentId, signature))
            {
                Db.Audit(userId, "payment_signature_invalid", booking.Pnr, ip, UserAgent);
                Toast("We could not verify that payment. If money was deducted it will be refunded automatically.", "danger");
                return;
            }

            // 2. ask Razorpay directly what happened - never trust the browser
            string err;
            GatewayPayment p = RazorpayService.FetchPayment(paymentId, out err);
            long expectedPaise = (long)Math.Round(booking.TotalAmount * 100m);
            if (p == null || p.OrderId != state.OrderId || p.AmountPaise != expectedPaise || p.Currency != "INR")
            {
                Db.Audit(userId, "payment_mismatch", booking.Pnr + " " + paymentId, ip, UserAgent);
                Toast("The payment details did not match this booking. Please contact support with reference " + booking.Pnr + ".", "danger");
                return;
            }
            if (p.Status == "authorized")
            {
                p = RazorpayService.Capture(paymentId, expectedPaise, out err) ?? p;
            }
            if (p.Status != "captured")
            {
                BookingService.MarkPayment(state.OrderId, "Failed", paymentId, "status " + p.Status, null);
                Toast("The payment was not completed (" + p.Status + "). You have not been charged. Please try again.", "danger");
                return;
            }

            // 3. confirm the seats in one database transaction
            int result = BookingService.Confirm(booking.Pnr, userId, state.OrderId, paymentId, p.Method, booking.TotalAmount);
            if (result == 1 || result == 2)
            {
                Db.Audit(userId, "booking_confirmed", booking.Pnr + " " + paymentId + " " + p.Method, ip, UserAgent);
                if (result == 1) EmailService.SendBookingConfirmed(booking.Pnr, userId);
                Session.Remove("Checkout");
                Response.Redirect("BookingConfirmation.aspx?pnr=" + booking.Pnr, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (result == -3)
            {
                // paid, but a seat is gone (hold lapsed and someone else got it) -> refund automatically
                string refundErr;
                string refundId = RazorpayService.Refund(paymentId, expectedPaise, out refundErr);
                BookingService.MarkPayment(state.OrderId, refundId != null ? "Refunded" : "RefundPending", paymentId, "seat no longer available", refundId);
                BookingService.CancelPending(booking.Pnr, userId);
                Db.Audit(userId, refundId != null ? "payment_refunded" : "payment_refund_pending", booking.Pnr + " " + paymentId, ip, UserAgent);
                EmailService.SendPaymentIssue(booking.ContactEmail, booking.Pnr,
                    "Sorry - the seats you paid for were released before payment completed and are no longer available. " +
                    (refundId != null ? "We have started a full refund." : "Our team will process a full refund."));
                Session.Remove("Checkout");
                Flash("The seats were no longer available, so your payment is being refunded in full.", "warning");
                Response.Redirect("Home.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Db.Audit(userId, "payment_confirm_failed", booking.Pnr + " code " + result, ip, UserAgent);
            Toast("We received your payment but could not finish the booking. Please contact support with reference " + booking.Pnr + ".", "danger");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (state != null)
            {
                if (!string.IsNullOrEmpty(state.Pnr)) BookingService.CancelPending(state.Pnr, AuthService.UserId);
                BookingService.ReleaseHolds(state.ScheduleId, AuthService.UserId);
            }
            Session.Remove("Checkout");
            Flash("Booking cancelled. No payment was taken.", "info");
            Response.Redirect("Home.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
