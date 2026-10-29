using System;
using System.Data;
using System.Web;
using System.Web.UI;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject.Admin
{
    public partial class AdminChangePassword : Page
    {
        private string AdminId
        {
            get { string n = User.Identity.Name ?? ""; return n.StartsWith("admin:") ? n.Substring(6) : n; }
        }

        private void Toast(string message, string type)
        {
            ClientScript.RegisterStartupScript(GetType(), "t" + Guid.NewGuid().ToString("N"),
                "(window.mbtQ=window.mbtQ||[]).push(['" + HttpUtility.JavaScriptStringEncode(message) + "','" + type + "']);", true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            object o = Db.Scalar("SELECT MustChangePassword FROM dbo.Admin_Login WHERE Admin_ID = @id", Db.P("@id", AdminId));
            pnlForced.Visible = o != null && Convert.ToBoolean(o);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string current = txtCurrent.Text ?? "", pw = txtNew.Text ?? "";
            if (pw != (txtConfirm.Text ?? "")) { Toast("The two new passwords do not match.", "danger"); return; }
            if (pw == current) { Toast("The new password must be different from the current one.", "danger"); return; }
            string policy = PasswordHasher.CheckPolicy(pw, null);
            if (policy != null) { Toast(policy, "danger"); return; }
            if (!Throttle.Allow("admin-pw:" + AdminId, 6, TimeSpan.FromMinutes(15))) { Toast("Too many attempts. Please wait.", "danger"); return; }

            DataTable dt = Db.Query("SELECT TOP 1 [Password], PasswordHash FROM dbo.Admin_Login WHERE Admin_ID = @id", Db.P("@id", AdminId));
            if (dt.Rows.Count == 0) { Toast("Account not found.", "danger"); return; }
            string hash = dt.Rows[0]["PasswordHash"] == DBNull.Value ? null : Convert.ToString(dt.Rows[0]["PasswordHash"]);
            string legacy = dt.Rows[0]["Password"] == DBNull.Value ? null : Convert.ToString(dt.Rows[0]["Password"]);
            bool rehash, ok = !string.IsNullOrEmpty(hash) ? PasswordHasher.Verify(current, hash, out rehash)
                                                          : (!string.IsNullOrEmpty(legacy) && Crypto.FixedTimeEquals(legacy, current));
            if (!ok) { Toast("Your current password is not correct.", "danger"); return; }

            Db.NonQuery("UPDATE dbo.Admin_Login SET PasswordHash = @h, [Password] = NULL, MustChangePassword = 0, FailedAttempts = 0, LockoutEnd = NULL WHERE Admin_ID = @id",
                Db.P("@h", PasswordHasher.Hash(pw)), Db.P("@id", AdminId));
            Db.Audit(null, "admin_password_changed", AdminId, Throttle.ClientIp(Request), Request.UserAgent);
            pnlForced.Visible = false;
            Toast("Password updated.", "success");
        }
    }
}
