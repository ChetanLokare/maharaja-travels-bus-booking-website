using System;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class ResetPassword : BasePage
    {
        private int userId;

        protected void Page_Load(object sender, EventArgs e)
        {
            object id = Session["ResetUserId"], until = Session["ResetUntil"];
            if (id == null || until == null || (DateTime)until < DateTime.Now)
            {
                Session.Remove("ResetUserId"); Session.Remove("ResetUntil");
                Flash("Please verify your code again to reset your password.", "warning");
                Response.Redirect("ForgotPassword.aspx", true);
                return;
            }
            userId = (int)id;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            UserRecord u = UserRepository.FindById(userId);
            if (u == null) { Response.Redirect("Login.aspx", true); return; }

            string pw = txtNew.Text ?? "";
            if (pw != (txtConfirm.Text ?? "")) { Toast("The two passwords do not match.", "danger"); return; }
            string error = PasswordHasher.CheckPolicy(pw, u.Email);
            if (error != null) { Toast(error, "danger"); return; }

            UserRepository.SetPassword(userId, PasswordHasher.Hash(pw));
            UserRepository.MarkEmailVerified(userId);     // the reset code went to this e-mail
            Db.Audit(userId, "password_reset", null, ClientIp, UserAgent);
            if (Validator.IsEmail(u.Email)) EmailService.SendPasswordChanged(u.Email, u.FirstName);

            Session.Remove("ResetUserId"); Session.Remove("ResetUntil");
            Response.Redirect("Login.aspx?reset=1", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
