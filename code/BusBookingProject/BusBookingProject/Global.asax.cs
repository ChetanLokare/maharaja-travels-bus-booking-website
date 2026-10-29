using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using BusBookingProject.Security;

namespace BusBookingProject
{
    public class Global : HttpApplication
    {
        private static readonly object LogLock = new object();

        protected void Application_Start(object sender, EventArgs e)
        {
            // Payment, e-mail and sign-in providers all require modern TLS.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            try { ServicePointManager.SecurityProtocol |= (SecurityProtocolType)12288; } catch { /* TLS 1.3 enum not available */ }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpRequest req = Request;
            bool secure = req.IsSecureConnection || string.Equals(req.Headers["X-Forwarded-Proto"], "https", StringComparison.OrdinalIgnoreCase) && Config.GetBool("Security:TrustProxyHeaders", false);

            if (!secure && !req.IsLocal && Config.GetBool("Security:RequireHttps", true)
                && !req.Url.AbsolutePath.EndsWith("PaymentWebhook.ashx", StringComparison.OrdinalIgnoreCase))
            {
                string url = "https://" + req.Url.Host + req.RawUrl;
                Response.StatusCode = 301;
                Response.RedirectLocation = url;
                CompleteRequest();
                return;
            }

            HttpResponse res = Response;
            res.AppendHeader("X-Content-Type-Options", "nosniff");
            res.AppendHeader("X-Frame-Options", "SAMEORIGIN");
            res.AppendHeader("Referrer-Policy", "strict-origin-when-cross-origin");
            res.AppendHeader("Permissions-Policy", "camera=(), microphone=(), geolocation=(), payment=(self \"https://checkout.razorpay.com\")");
            res.AppendHeader("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
            res.AppendHeader("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://checkout.razorpay.com; " +
                "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
                "font-src 'self' https://cdn.jsdelivr.net data:; " +
                "img-src 'self' data: https:; " +
                "connect-src 'self' https://api.razorpay.com https://lumberjack.razorpay.com https://checkout.razorpay.com; " +
                "frame-src https://api.razorpay.com https://checkout.razorpay.com; " +
                "object-src 'none'; base-uri 'self'; frame-ancestors 'self'; form-action 'self'");
            if (req.IsSecureConnection)
                res.AppendHeader("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        }

        /// <summary>Copy the signed role out of the forms-authentication ticket and enforce a 12 hour absolute session limit.</summary>
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            FormsIdentity id = Context.User != null ? Context.User.Identity as FormsIdentity : null;
            if (id == null || !id.IsAuthenticated || id.Ticket == null) return;

            string[] parts = (id.Ticket.UserData ?? "").Split('|');   // role | displayName | loginTicks
            string role = parts.Length > 0 && parts[0].Length > 0 ? parts[0] : AuthService.RoleUser;

            long ticks;
            if (parts.Length > 2 && long.TryParse(parts[2], out ticks))
            {
                if (DateTime.UtcNow - new DateTime(ticks, DateTimeKind.Utc) > TimeSpan.FromHours(12))
                {
                    FormsAuthentication.SignOut();
                    Context.User = new GenericPrincipal(new GenericIdentity(string.Empty), new string[0]);
                    return;
                }
            }
            Context.User = new GenericPrincipal(id, new[] { role });
        }

        /// <summary>The existing admin pages read Session["UserName"]; keep that in step with the signed ticket.</summary>
        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            if (Context.Session == null || Context.User == null || !Context.User.Identity.IsAuthenticated) return;
            if (!Context.User.IsInRole(AuthService.RoleAdmin)) return;
            FormsIdentity id = Context.User.Identity as FormsIdentity;
            if (id == null) return;
            if (Context.Session["UserName"] == null)
            {
                string[] parts = (id.Ticket.UserData ?? "").Split('|');
                Context.Session["UserName"] = parts.Length > 1 ? parts[1] : "Admin";
                Context.Session["UserId"] = id.Name.StartsWith("admin:") ? id.Name.Substring(6) : id.Name;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex == null) return;
            if (ex is HttpException && ((HttpException)ex).GetHttpCode() == 404) return;   // let customErrors handle 404

            string reference = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpperInvariant();
            try
            {
                string path = Server.MapPath("~/App_Data/errors.log");
                var sb = new StringBuilder();
                sb.AppendLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ref=" + reference + " url=" + Request.Url.AbsolutePath);
                sb.AppendLine(ex.GetBaseException().ToString());
                sb.AppendLine(new string('-', 60));
                lock (LogLock) { File.AppendAllText(path, sb.ToString()); }
            }
            catch { /* logging must never throw */ }

            Context.Items["ErrorRef"] = reference;
        }
    }
}
