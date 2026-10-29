using System;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class OtpVerify : BasePage
    {
        protected int ResendWait = 45;
        private OtpContext ctx;

        protected void Page_Load(object sender, EventArgs e)
        {
            ctx = Session["Otp"] as OtpContext;
            if (ctx == null || ctx.IsExpired)
            {
                Session.Remove("Otp");
                Flash("That verification has expired. Please start again.", "warning");
                Response.Redirect("Login.aspx", true);
                return;
            }
            ShowDevCode();
            if (!IsPostBack)
                litTarget.Text = Server.HtmlEncode(ctx.Channel == "sms" ? Validator.MaskMobile(ctx.Target) : ctx.Target.StartsWith("none:") ? "your registered contact" : Validator.MaskEmail(ctx.Target));
        }

        private void ShowDevCode()
        {
            string code = Session["DevOtp"] as string;
            if (EmailService.IsDevMode && !string.IsNullOrEmpty(code))
            {
                litDevCode.Text = Server.HtmlEncode(code);
                pnlDev.Visible = true;
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            if (!Throttle.Allow("otpv:" + ctx.Target, 12, TimeSpan.FromMinutes(15)) || !Throttle.Allow("otpv-ip:" + ClientIp, 30, TimeSpan.FromMinutes(15)))
            {
                Toast("Too many attempts. Please wait a few minutes.", "danger");
                return;
            }

            string error;
            if (!OtpService.Verify(ctx.Purpose, ctx.Target, txtCode.Text, out error))
            {
                Db.Audit(ctx.UserId == 0 ? (int?)null : ctx.UserId, "otp_failed", ctx.Purpose, ClientIp, UserAgent);
                txtCode.Text = string.Empty;
                Toast(error, "danger");
                return;
            }

            switch (ctx.Purpose)
            {
                case OtpService.Login: FinishLogin(); break;
                case OtpService.Register: FinishRegistration(); break;
                case OtpService.Reset: FinishReset(); break;
                case OtpService.Link: FinishLink(); break;
                default: Response.Redirect("Login.aspx", true); break;
            }
        }

        private void FinishLogin()
        {
            UserRecord u = UserRepository.FindById(ctx.UserId);
            if (u == null || !u.IsActive) { Toast("This account cannot be used to sign in.", "danger"); return; }
            if (ctx.Channel == "email" && !u.EmailVerified) UserRepository.MarkEmailVerified(u.UserId);
            CompleteSignIn(u, ctx.Method ?? "One-time code", ctx.ReturnUrl);
        }

        private void FinishRegistration()
        {
            if (UserRepository.EmailOrMobileExists(ctx.Email, ctx.Mobile))
            {
                Session.Remove("Otp");
                Flash("An account with these details already exists. Please sign in.", "warning");
                Response.Redirect("Login.aspx", true);
                return;
            }
            int id = UserRepository.Create(ctx.FirstName, ctx.LastName, ctx.Email, ctx.Mobile, ctx.Address, ctx.City, ctx.Pincode, ctx.PasswordHash, true);
            UserRecord u = UserRepository.FindById(id);
            Db.Audit(id, "registered", "e-mail verified", ClientIp, UserAgent);
            EmailService.SendWelcome(u.Email, u.FirstName);
            Flash("Welcome aboard, " + u.FirstName + "! Your account is ready.", "success");
            CompleteSignIn(u, "New account", ctx.ReturnUrl);
        }

        private void FinishReset()
        {
            if (ctx.UserId <= 0) { Toast("This reset request is not valid.", "danger"); return; }
            Session.Remove("DevOtp");
            Session["ResetUserId"] = ctx.UserId;
            Session["ResetUntil"] = DateTime.Now.AddMinutes(10);
            Session.Remove("Otp");
            Response.Redirect("ResetPassword.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void FinishLink()
        {
            UserRecord u = ctx.UserId > 0 ? UserRepository.FindById(ctx.UserId) : null;
            if (u == null)
            {
                if (UserRepository.EmailOrMobileExists(ctx.Email, null))
                {
                    u = UserRepository.FindByIdentifier(ctx.Email);
                }
                else
                {
                    int id = UserRepository.Create(ctx.FirstName, ctx.LastName, ctx.Email, null, null, null, null, null, true);
                    u = UserRepository.FindById(id);
                    EmailService.SendWelcome(u.Email, u.FirstName);
                }
            }
            if (u == null || !u.IsActive) { Toast("This account cannot be used to sign in.", "danger"); return; }
            if (!u.EmailVerified) UserRepository.MarkEmailVerified(u.UserId);
            UserRepository.LinkExternal(ctx.Provider, ctx.ProviderUserId, u.UserId, ctx.Email);
            CompleteSignIn(u, ctx.Method ?? "Social sign-in", ctx.ReturnUrl);
        }

        protected void btnResend_Click(object sender, EventArgs e)
        {
            if (ctx.Target.StartsWith("none:")) { Toast("A new code has been sent if the account exists.", "info"); return; }
            string name = ctx.FirstName ?? "there";
            OtpResult r = OtpService.Issue(ctx.Purpose, ctx.Target, ctx.Channel, name, ClientIp);
            if (!r.Ok) { Toast(r.Error, "warning"); ResendWait = r.RetryAfterSeconds; return; }
            ctx.ExpiresAt = DateTime.Now.AddMinutes(20);
            ResendWait = 60;
            ShowDevCode();
            Toast("A new code is on its way.", "success");
        }
    }
}
