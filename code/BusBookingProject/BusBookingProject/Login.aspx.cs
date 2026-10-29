using System;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class Login : BasePage
    {
        protected bool GoogleEnabled { get { return OAuthService.IsConfigured(OAuthService.Google); } }
        protected bool MicrosoftEnabled { get { return OAuthService.IsConfigured(OAuthService.Microsoft); } }

        private string ReturnUrl { get { return AuthService.SafeReturnUrl(Request.QueryString["ReturnUrl"], null); } }
        protected string ReturnUrlEncoded { get { return Server.UrlEncode(ReturnUrl ?? ""); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            string ret = Request.QueryString["ReturnUrl"] ?? "";
            if (ret.IndexOf("/Admin", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Response.Redirect("Admin/AdminLogin.aspx", true);
                return;
            }
            if (AuthService.IsUser) { Response.Redirect("Home.aspx", true); return; }

            if (Request.QueryString["loggedout"] == "1") Toast("You have been signed out.", "success");
            if (Request.QueryString["reset"] == "1") Toast("Password updated. Please sign in with your new password.", "success");
        }

        // ------------------------------------------------------------------ password
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            hdnTab.Value = "pw";
            string id = Validator.Clean(txtUserId.Text, 150);
            string password = txtPassword.Text ?? string.Empty;
            txtPassword.Text = string.Empty;

            if (id.Length == 0 || password.Length == 0) { Toast("Enter your e-mail or mobile number and your password.", "warning"); return; }

            if (!Throttle.Allow("login-ip:" + ClientIp, 15, TimeSpan.FromMinutes(15)) ||
                !Throttle.Allow("login-id:" + id.ToLowerInvariant(), 10, TimeSpan.FromMinutes(15)))
            {
                Toast("Too many sign-in attempts. Please wait a few minutes and try again.", "danger");
                return;
            }

            const string generic = "Incorrect e-mail/mobile or password. If you signed up with Google or Microsoft, use that button or the one-time code tab.";
            UserRecord u = UserRepository.FindByIdentifier(id);
            if (u == null || !u.IsActive)
            {
                PasswordHasher.BurnTime();          // same cost as a real check - no account enumeration by timing
                Db.Audit(null, "login_failed", "unknown account", ClientIp, UserAgent);
                Toast(generic, "danger");
                return;
            }

            if (u.LockoutEnd.HasValue && u.LockoutEnd.Value <= DateTime.Now)
            {
                UserRepository.ResetLockout(u.UserId);
                u.FailedAttempts = 0; u.LockoutEnd = null;
            }
            if (u.IsLocked)
            {
                int mins = Math.Max(1, (int)Math.Ceiling((u.LockoutEnd.Value - DateTime.Now).TotalMinutes));
                Toast("This account is temporarily locked after repeated failed attempts. Try again in " + mins + " minute(s), or reset your password.", "danger");
                return;
            }

            bool ok = false, needsRehash = false;
            if (!string.IsNullOrEmpty(u.PasswordHash))
            {
                ok = PasswordHasher.Verify(password, u.PasswordHash, out needsRehash);
            }
            else if (!string.IsNullOrEmpty(u.LegacyPassword))
            {
                // account created before the security upgrade: accept once, then store only a salted hash
                ok = Crypto.FixedTimeEquals(u.LegacyPassword, password);
                needsRehash = ok;
            }
            else
            {
                PasswordHasher.BurnTime();
            }

            if (!ok)
            {
                UserRepository.RecordFailure(u.UserId);
                Db.Audit(u.UserId, "login_failed", "bad password", ClientIp, UserAgent);
                Toast(generic, "danger");
                return;
            }
            if (needsRehash) UserRepository.SetPassword(u.UserId, PasswordHasher.Hash(password));

            if (Config.GetBool("Security:TwoStepForPasswordLogin", true) && Validator.IsEmail(u.Email))
            {
                StartOtp(u, u.Email, "email", "Password + e-mail code");
                return;
            }
            CompleteSignIn(u, "Password", ReturnUrl);
        }

        // ------------------------------------------------------------------ one-time code
        protected void btnSendOtp_Click(object sender, EventArgs e)
        {
            hdnTab.Value = "otp";
            string id = Validator.Clean(txtOtpId.Text, 150);
            if (id.Length == 0) { Toast("Enter your e-mail or mobile number.", "warning"); return; }

            if (!Throttle.Allow("otp-ip:" + ClientIp, 10, TimeSpan.FromHours(1)))
            {
                Toast("Too many code requests. Please try again later.", "danger");
                return;
            }

            UserRecord u = UserRepository.FindByIdentifier(id);
            if (u == null || !u.IsActive)
            {
                // identical response either way, so nobody can probe which accounts exist
                var fake = new OtpContext { Purpose = OtpService.Login, Target = "none:" + Crypto.RandomToken(6), Channel = "email", ReturnUrl = ReturnUrl, Method = "One-time code" };
                Session["Otp"] = fake;
                Flash("If an account exists, we have sent a 6-digit code to it.", "info");
                Response.Redirect("OtpVerify.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string mobile = Validator.NormalizeMobile(u.Mobile);
            if (!id.Contains("@") && SmsService.IsEnabled && mobile != null)
                StartOtp(u, mobile, "sms", "One-time code (SMS)");
            else if (Validator.IsEmail(u.Email))
                StartOtp(u, u.Email, "email", "One-time code (e-mail)");
            else
                Toast("We have no way to send a code to this account. Please use your password.", "danger");
        }

        private void StartOtp(UserRecord u, string target, string channel, string method)
        {
            OtpResult r = OtpService.Issue(OtpService.Login, target, channel, u.FirstName, ClientIp);
            if (!r.Ok) { Toast(r.Error, "danger"); return; }

            Session["Otp"] = new OtpContext
            {
                Purpose = OtpService.Login, Target = target, Channel = channel, UserId = u.UserId,
                FirstName = u.FirstName, ReturnUrl = ReturnUrl, Method = method
            };
            Response.Redirect("OtpVerify.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
