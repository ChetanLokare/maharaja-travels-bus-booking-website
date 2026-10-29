using System;
using System.Web;
using System.Web.SessionState;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    [Serializable]
    public class OAuthSession
    {
        public string Provider, State, Nonce, Verifier, ReturnUrl;
        public DateTime Created = DateTime.Now;
    }

    /// <summary>Begins Google / Microsoft sign-in (authorization-code flow with PKCE, state and nonce).</summary>
    public class OAuthStart : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            string provider = OAuthService.Normalize(context.Request.QueryString["provider"]);
            if (provider == null || !OAuthService.IsConfigured(provider))
            {
                context.Response.Redirect("~/Login.aspx", false);
                context.ApplicationInstance.CompleteRequest();
                return;
            }
            if (!Throttle.Allow("oauth-ip:" + Throttle.ClientIp(context.Request), 20, TimeSpan.FromMinutes(15)))
            {
                context.Response.StatusCode = 429;
                return;
            }

            var s = new OAuthSession
            {
                Provider = provider,
                State = Crypto.RandomToken(32),
                Nonce = Crypto.RandomToken(32),
                Verifier = Crypto.RandomToken(48),
                ReturnUrl = AuthService.SafeReturnUrl(context.Request.QueryString["ReturnUrl"], null)
            };
            context.Session["oauth"] = s;

            string url = OAuthService.BuildAuthorizeUrl(provider, s.State, s.Nonce, OAuthService.CodeChallenge(s.Verifier));
            context.Response.Redirect(url, false);
            context.ApplicationInstance.CompleteRequest();
        }
    }
}
