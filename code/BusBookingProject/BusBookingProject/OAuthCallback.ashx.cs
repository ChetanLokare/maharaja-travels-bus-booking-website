using System;
using System.Web;
using System.Web.SessionState;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    /// <summary>Receives the provider's redirect, validates it, then signs in / links / creates the account.</summary>
    public class OAuthCallback : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return false; } }

        private static void Fail(HttpContext ctx, string message)
        {
            var list = ctx.Session["flash"] as System.Collections.Generic.List<string[]>;
            if (list == null) { list = new System.Collections.Generic.List<string[]>(); ctx.Session["flash"] = list; }
            list.Add(new[] { message, "danger" });
            ctx.Response.Redirect("~/Login.aspx", false);
            ctx.ApplicationInstance.CompleteRequest();
        }

        public void ProcessRequest(HttpContext ctx)
        {
            var s = ctx.Session["oauth"] as OAuthSession;
            ctx.Session.Remove("oauth");                                   // single use
            string state = ctx.Request.QueryString["state"], code = ctx.Request.QueryString["code"];

            if (s == null || (DateTime.Now - s.Created).TotalMinutes > 10 || string.IsNullOrEmpty(state) || !Crypto.FixedTimeEquals(state, s.State))
            { Fail(ctx, "Your sign-in session expired. Please try again."); return; }
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["error"]) || string.IsNullOrEmpty(code))
            { Fail(ctx, "Sign-in was cancelled or refused by the provider."); return; }

            string error;
            OAuthProfile p = OAuthService.Exchange(s.Provider, code, s.Verifier, s.Nonce, out error);
            if (p == null) { Fail(ctx, error ?? "Sign-in failed."); return; }

            string label = s.Provider == OAuthService.Google ? "Google" : "Microsoft";
            string ip = Throttle.ClientIp(ctx.Request), ua = ctx.Request.UserAgent ?? "";

            // 1. this provider account is already linked
            int? linked = UserRepository.FindByExternal(p.Provider, p.Subject);
            if (linked.HasValue)
            {
                UserRecord lu = UserRepository.FindById(linked.Value);
                if (lu == null || !lu.IsActive) { Fail(ctx, "This account is not available."); return; }
                SignInFlow.Complete(ctx, lu, label + " sign-in", s.ReturnUrl);
                return;
            }

            UserRecord existing = UserRepository.FindByIdentifier(p.Email);

            // 2. provider vouches for the e-mail (Google) -> link or create straight away
            if (p.EmailVerified)
            {
                UserRecord u = existing;
                if (u == null)
                {
                    int id = UserRepository.Create(p.FirstName, p.LastName, p.Email, null, null, null, null, null, true);
                    u = UserRepository.FindById(id);
                    Db.Audit(id, "registered", label, ip, ua);
                    EmailService.SendWelcome(u.Email, u.FirstName);
                }
                else if (!u.IsActive) { Fail(ctx, "This account is not available."); return; }
                else if (!u.EmailVerified) UserRepository.MarkEmailVerified(u.UserId);

                UserRepository.LinkExternal(p.Provider, p.Subject, u.UserId, p.Email);
                SignInFlow.Complete(ctx, u, label + " sign-in", s.ReturnUrl);
                return;
            }

            // 3. e-mail not verified by the provider -> prove ownership with an e-mail code before linking/creating
            OtpResult r = OtpService.Issue(OtpService.Link, p.Email, "email", p.FirstName, ip);
            if (!r.Ok) { Fail(ctx, r.Error); return; }
            ctx.Session["Otp"] = new OtpContext
            {
                Purpose = OtpService.Link, Target = p.Email, Channel = "email", Method = label + " sign-in",
                UserId = existing != null ? existing.UserId : 0, FirstName = p.FirstName, LastName = p.LastName, Email = p.Email,
                Provider = p.Provider, ProviderUserId = p.Subject, ReturnUrl = s.ReturnUrl
            };
            ctx.Response.Redirect("~/OtpVerify.aspx", false);
            ctx.ApplicationInstance.CompleteRequest();
        }
    }
}
