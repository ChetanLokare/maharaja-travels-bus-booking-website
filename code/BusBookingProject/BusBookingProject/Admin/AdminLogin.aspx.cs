using System;
using System.Data;
using System.Web.UI;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject.Admin
{
    public partial class AdminLogin : Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (Session != null)
            {
                if (Session["_vsk"] == null) Session["_vsk"] = Guid.NewGuid().ToString("N");
                ViewStateUserKey = (string)Session["_vsk"];
            }
            base.OnInit(e);
        }

        private void Toast(string message, string type)
        {
            ClientScript.RegisterStartupScript(GetType(), "t" + Guid.NewGuid().ToString("N"),
                "(window.mbtQ=window.mbtQ||[]).push(['" + System.Web.HttpUtility.JavaScriptStringEncode(message) + "','" + type + "']);", true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && AuthService.IsAdmin) Response.Redirect("Default.aspx", true);
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string ip = Throttle.ClientIp(Request), ua = Request.UserAgent ?? "";
            string id = Validator.Clean(txtUserId.Text, 50);
            string password = txtPassword.Text ?? "";
            txtPassword.Text = string.Empty;
            const string generic = "Incorrect admin ID or password.";

            if (id.Length == 0 || password.Length == 0) { Toast("Enter your admin ID and password.", "warning"); return; }
            if (!Throttle.Allow("admin-ip:" + ip, 8, TimeSpan.FromMinutes(15)) || !Throttle.Allow("admin-id:" + id.ToLowerInvariant(), 8, TimeSpan.FromMinutes(15)))
            {
                Toast("Too many attempts. Please wait before trying again.", "danger");
                return;
            }

            DataTable dt = Db.Query(
                "SELECT TOP 1 Admin_ID, UserName, [Password], PasswordHash, FailedAttempts, LockoutEnd, MustChangePassword FROM dbo.Admin_Login WHERE Admin_ID = @id",
                Db.P("@id", id));
            if (dt.Rows.Count == 0)
            {
                PasswordHasher.BurnTime();
                Db.Audit(null, "admin_login_failed", "unknown admin " + id, ip, ua);
                Toast(generic, "danger");
                return;
            }

            DataRow r = dt.Rows[0];
            DateTime? lockEnd = r["LockoutEnd"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["LockoutEnd"]);
            if (lockEnd.HasValue && lockEnd.Value > DateTime.Now)
            {
                Toast("This account is temporarily locked after repeated failures. Try again later.", "danger");
                return;
            }

            string hash = r["PasswordHash"] == DBNull.Value ? null : Convert.ToString(r["PasswordHash"]);
            string legacy = r["Password"] == DBNull.Value ? null : Convert.ToString(r["Password"]);
            bool ok = false, rehash = false, mustChange = Convert.ToBoolean(r["MustChangePassword"]);

            if (!string.IsNullOrEmpty(hash)) ok = PasswordHasher.Verify(password, hash, out rehash);
            else if (!string.IsNullOrEmpty(legacy)) { ok = Crypto.FixedTimeEquals(legacy, password); rehash = ok; mustChange = true; }
            else PasswordHasher.BurnTime();

            if (!ok)
            {
                int max = Config.GetInt("Security:MaxFailedLogins", 5), minutes = Config.GetInt("Security:LockoutMinutes", 15);
                Db.NonQuery(@"UPDATE dbo.Admin_Login
                              SET FailedAttempts = CASE WHEN LockoutEnd IS NOT NULL AND LockoutEnd <= GETDATE() THEN 1 ELSE FailedAttempts + 1 END,
                                  LockoutEnd = CASE WHEN (CASE WHEN LockoutEnd IS NOT NULL AND LockoutEnd <= GETDATE() THEN 1 ELSE FailedAttempts + 1 END) >= @max
                                               THEN DATEADD(MINUTE, @min, GETDATE()) ELSE NULL END
                              WHERE Admin_ID = @id", Db.P("@max", max), Db.P("@min", minutes), Db.P("@id", id));
                Db.Audit(null, "admin_login_failed", "bad password for " + id, ip, ua);
                Toast(generic, "danger");
                return;
            }

            if (rehash)
                Db.NonQuery("UPDATE dbo.Admin_Login SET PasswordHash = @h, [Password] = NULL WHERE Admin_ID = @id", Db.P("@h", PasswordHasher.Hash(password)), Db.P("@id", id));
            Db.NonQuery("UPDATE dbo.Admin_Login SET FailedAttempts = 0, LockoutEnd = NULL, LastLoginAt = GETDATE() WHERE Admin_ID = @id", Db.P("@id", id));

            string name = Convert.ToString(r["UserName"]);
            AuthService.SignIn(Context, "admin:" + id, name, AuthService.RoleAdmin);
            Session["UserId"] = id;
            Session["UserName"] = name;
            Db.Audit(null, "admin_login", id, ip, ua);

            Response.Redirect(mustChange ? "AdminChangePassword.aspx" : "Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
