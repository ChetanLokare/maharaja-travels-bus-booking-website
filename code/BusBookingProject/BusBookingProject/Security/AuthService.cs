using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace BusBookingProject.Security
{
    /// <summary>
    /// Sign-in / sign-out built on encrypted + signed forms-authentication tickets (HttpOnly, SameSite=Lax, Secure on HTTPS).
    /// The ticket carries  role | display name | login time.  Customer identity name is the numeric regId;
    /// administrators use "admin:&lt;Admin_ID&gt;".
    /// </summary>
    public static class AuthService
    {
        public const string RoleUser = "User";
        public const string RoleAdmin = "Admin";

        public static void SignIn(HttpContext ctx, string identityName, string displayName, string role)
        {
            string safeName = (displayName ?? "").Replace("|", " ");
            int minutes = 30;
            var ticket = new FormsAuthenticationTicket(
                2, identityName, DateTime.Now, DateTime.Now.AddMinutes(minutes), false,
                role + "|" + safeName + "|" + DateTime.UtcNow.Ticks, FormsAuthentication.FormsCookiePath);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL || ctx.Request.IsSecureConnection,
                Path = FormsAuthentication.FormsCookiePath,
                SameSite = SameSiteMode.Lax
            };
            ctx.Response.Cookies.Add(cookie);
            ctx.User = new GenericPrincipal(new FormsIdentity(ticket), new[] { role });
        }

        public static void SignOut(HttpContext ctx)
        {
            FormsAuthentication.SignOut();
            if (ctx.Session != null)
            {
                ctx.Session.Clear();
                ctx.Session.Abandon();
            }
            ExpireCookie(ctx, FormsAuthentication.FormsCookieName);
            ExpireCookie(ctx, "MBT_SESSION");
            ctx.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            ctx.Response.Cache.SetNoStore();
        }

        private static void ExpireCookie(HttpContext ctx, string name)
        {
            var c = new HttpCookie(name, string.Empty) { Expires = DateTime.UtcNow.AddYears(-1), HttpOnly = true, Path = "/" };
            ctx.Response.Cookies.Add(c);
        }

        // ---- current identity ----
        private static IPrincipal Principal { get { return HttpContext.Current != null ? HttpContext.Current.User : null; } }

        public static bool IsUser
        {
            get { var p = Principal; return p != null && p.Identity.IsAuthenticated && p.IsInRole(RoleUser); }
        }

        public static bool IsAdmin
        {
            get { var p = Principal; return p != null && p.Identity.IsAuthenticated && p.IsInRole(RoleAdmin); }
        }

        public static int UserId
        {
            get
            {
                int id;
                return IsUser && int.TryParse(Principal.Identity.Name, out id) ? id : 0;
            }
        }

        public static string DisplayName
        {
            get
            {
                var id = Principal != null ? Principal.Identity as FormsIdentity : null;
                if (id == null || id.Ticket == null) return string.Empty;
                string[] parts = (id.Ticket.UserData ?? "").Split('|');
                return parts.Length > 1 ? parts[1] : string.Empty;
            }
        }

        /// <summary>Only same-site, app-relative paths may be used as a post-login destination (blocks open redirects).</summary>
        public static string SafeReturnUrl(string url, string fallback)
        {
            if (string.IsNullOrWhiteSpace(url)) return fallback;
            url = url.Trim();
            if (url[0] != '/' || url.StartsWith("//") || url.StartsWith("/\\") || url.IndexOf('\\') >= 0
                || url.IndexOf('\r') >= 0 || url.IndexOf('\n') >= 0)
                return fallback;
            Uri parsed;
            return Uri.TryCreate(url, UriKind.Relative, out parsed) ? url : fallback;
        }
    }
}
