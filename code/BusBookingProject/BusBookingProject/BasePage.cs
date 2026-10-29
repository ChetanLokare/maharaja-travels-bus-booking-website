using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    /// <summary>Common behaviour for every public page: CSRF binding for ViewState, login guard, flash messages.</summary>
    public class BasePage : Page
    {
        protected virtual bool RequiresLogin { get { return false; } }

        protected override void OnInit(EventArgs e)
        {
            if (RequiresLogin && !AuthService.IsUser)
            {
                Response.Redirect("~/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.RawUrl), true);
                return;
            }

            // Bind the signed ViewState to this browser session -> forged cross-site postbacks are rejected.
            if (Session != null)
            {
                if (Session["_vsk"] == null) Session["_vsk"] = Guid.NewGuid().ToString("N");
                ViewStateUserKey = (string)Session["_vsk"];
            }

            if (RequiresLogin)
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
            }
            base.OnInit(e);
        }

        // ---- messages shown as toasts (this page, or the next one after a redirect) ----
        protected void Toast(string message, string type = "info")
        {
            ClientScript.RegisterStartupScript(GetType(), "toast" + Guid.NewGuid().ToString("N"),
                "(window.mbtQ=window.mbtQ||[]).push(['" + HttpUtility.JavaScriptStringEncode(message) + "','" + type + "']);", true);
        }

        protected void Flash(string message, string type = "info")
        {
            var list = Session["flash"] as List<string[]>;
            if (list == null) { list = new List<string[]>(); Session["flash"] = list; }
            list.Add(new[] { message, type });
        }

        protected string ClientIp { get { return Throttle.ClientIp(Request); } }
        protected string UserAgent { get { return Request.UserAgent ?? string.Empty; } }

        /// <summary>Completes a sign-in for a customer: ticket, audit trail, alert e-mail, redirect.</summary>
        protected void CompleteSignIn(UserRecord u, string method, string returnUrl)
        {
            SignInFlow.Complete(Context, u, method, returnUrl);
        }
    }
}
