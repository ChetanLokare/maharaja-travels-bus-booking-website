using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using BusBookingProject.Security;

namespace BusBookingProject
{
    public partial class BusBookingMaster : MasterPage
    {
        protected bool IsLoggedIn { get { return AuthService.IsUser; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkSiteCss.Href = Asset("~/css/site.css");
            if (IsLoggedIn)
            {
                string name = AuthService.DisplayName;
                lblName.Text = HttpUtility.HtmlEncode(name.Length > 18 ? name.Substring(0, 18) + "..." : name);
                litInitial.Text = HttpUtility.HtmlEncode(name.Length > 0 ? name.Substring(0, 1).ToUpperInvariant() : "U");
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var flash = Session["flash"] as List<string[]>;
            if (flash == null || flash.Count == 0) return;
            foreach (string[] f in flash)
                Page.ClientScript.RegisterStartupScript(GetType(), "flash" + Guid.NewGuid().ToString("N"),
                    "(window.mbtQ=window.mbtQ||[]).push(['" + HttpUtility.JavaScriptStringEncode(f[0]) + "','" + f[1] + "']);", true);
            Session.Remove("flash");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthService.SignOut(Context);
            Response.Redirect("~/Login.aspx?loggedout=1", true);
        }

        protected string NavActive(string page)
        {
            string path = Request.Url.AbsolutePath;
            return path.EndsWith("/" + page, StringComparison.OrdinalIgnoreCase) ? " active" : string.Empty;
        }

        /// <summary>Static asset URL with a cache-busting version stamp.</summary>
        protected string Asset(string virtualPath)
        {
            string url = ResolveUrl(virtualPath);
            try
            {
                string file = Server.MapPath(virtualPath);
                if (File.Exists(file)) url += "?v=" + File.GetLastWriteTimeUtc(file).Ticks.ToString("x");
            }
            catch { }
            return url;
        }
    }
}
