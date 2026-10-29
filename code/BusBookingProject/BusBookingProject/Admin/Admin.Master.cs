using System;
using System.IO;
using System.Web;
using System.Web.UI;
using BusBookingProject.Security;

namespace BusBookingProject.Admin
{
    public partial class Admin : MasterPage
    {
        protected bool IsAdminSignedIn { get { return AuthService.IsAdmin; } }

        protected override void OnInit(EventArgs e)
        {
            string path = Request.Url.AbsolutePath;
            bool isLogin = path.EndsWith("/AdminLogin.aspx", StringComparison.OrdinalIgnoreCase);

            // defence in depth - web.config already denies anonymous access to /Admin
            if (!isLogin && !AuthService.IsAdmin)
            {
                Response.Redirect("~/Admin/AdminLogin.aspx", true);
                return;
            }

            // bind signed ViewState to this browser session (CSRF) for every admin page
            if (Session != null)
            {
                if (Session["_vsk"] == null) Session["_vsk"] = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = (string)Session["_vsk"];
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkSiteCss.Href = Asset("~/css/site.css");
            if (IsAdminSignedIn) lblAdmin.Text = HttpUtility.HtmlEncode(AuthService.DisplayName);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthService.SignOut(Context);
            Response.Redirect("~/Admin/AdminLogin.aspx", true);
        }

        protected string NavActive(string page)
        {
            return Request.Url.AbsolutePath.EndsWith("/" + page, StringComparison.OrdinalIgnoreCase) ? " active" : string.Empty;
        }

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
