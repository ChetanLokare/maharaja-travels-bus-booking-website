using System;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class ForgotPassword : BasePage
    {
        protected void btnSend_Click(object sender, EventArgs e)
        {
            string id = Validator.Clean(txtId.Text, 150);
            if (id.Length == 0) return;
            if (!Throttle.Allow("reset-ip:" + ClientIp, 8, TimeSpan.FromHours(1)))
            {
                Toast("Too many requests. Please try again later.", "danger");
                return;
            }

            UserRecord u = UserRepository.FindByIdentifier(id);
            var ctx = new OtpContext { Purpose = OtpService.Reset, Channel = "email", Method = "Password reset" };
            if (u != null && u.IsActive && Validator.IsEmail(u.Email))
            {
                OtpResult r = OtpService.Issue(OtpService.Reset, u.Email, "email", u.FirstName, ClientIp);
                if (!r.Ok && r.RetryAfterSeconds == 0) { Toast(r.Error, "danger"); return; }
                ctx.Target = u.Email; ctx.UserId = u.UserId; ctx.FirstName = u.FirstName;
                Db.Audit(u.UserId, "reset_requested", null, ClientIp, UserAgent);
            }
            else
            {
                ctx.Target = "none:" + Crypto.RandomToken(6);   // same screens and wording whether or not the account exists
            }
            Session["Otp"] = ctx;
            Flash("If an account exists, we have sent a 6-digit code to its e-mail address.", "info");
            Response.Redirect("OtpVerify.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
