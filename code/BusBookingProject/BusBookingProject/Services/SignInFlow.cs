using System;
using System.Web;
using BusBookingProject.Security;

namespace BusBookingProject.Services
{
    /// <summary>Pending multi-step sign-in / sign-up, kept server-side in the session (never in the browser).</summary>
    [Serializable]
    public class OtpContext
    {
        public string Purpose, Target, Channel, ReturnUrl, Method;
        public int UserId;
        public DateTime ExpiresAt = DateTime.Now.AddMinutes(20);

        // registration
        public string FirstName, LastName, Email, Mobile, Address, City, Pincode, PasswordHash;
        // social-account link
        public string Provider, ProviderUserId;

        public bool IsExpired { get { return DateTime.Now > ExpiresAt; } }
    }

    public static class SignInFlow
    {
        public static string UserAgentLabel(string ua)
        {
            if (string.IsNullOrEmpty(ua)) return "Unknown device";
            string os = ua.Contains("Windows") ? "Windows" : ua.Contains("Android") ? "Android" : ua.Contains("iPhone") || ua.Contains("iPad") ? "iOS"
                      : ua.Contains("Mac OS") ? "macOS" : ua.Contains("Linux") ? "Linux" : "Unknown OS";
            string br = ua.Contains("Edg/") ? "Edge" : ua.Contains("OPR/") ? "Opera" : ua.Contains("Chrome/") ? "Chrome" : ua.Contains("Firefox/") ? "Firefox"
                      : ua.Contains("Safari/") ? "Safari" : "Browser";
            return br + " on " + os;
        }

        /// <summary>Signs the customer in, records it, e-mails a sign-in confirmation and redirects.</summary>
        public static void Complete(HttpContext ctx, UserRecord u, string method, string returnUrl)
        {
            string ip = Throttle.ClientIp(ctx.Request);
            string ua = ctx.Request.UserAgent ?? string.Empty;

            UserRepository.RecordSuccess(u.UserId, ip);
            AuthService.SignIn(ctx, u.UserId.ToString(), u.DisplayName, AuthService.RoleUser);
            Db.Audit(u.UserId, "login", method, ip, ua);
            if (Validator.IsEmail(u.Email)) EmailService.SendLoginAlert(u.Email, u.FirstName, method, ip, UserAgentLabel(ua));

            if (ctx.Session != null)
            {
                ctx.Session.Remove("Otp");
                ctx.Session.Remove("DevOtp");
                ctx.Session.Remove("oauth");
            }
            string dest = AuthService.SafeReturnUrl(returnUrl, VirtualPathUtility.ToAbsolute("~/Home.aspx"));
            ctx.Response.Redirect(dest, false);
            ctx.ApplicationInstance.CompleteRequest();
        }
    }
}
