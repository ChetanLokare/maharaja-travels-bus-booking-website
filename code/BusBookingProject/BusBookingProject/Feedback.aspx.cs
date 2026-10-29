using System;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class Feedback : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack || !AuthService.IsUser) return;
            UserRecord u = UserRepository.FindById(AuthService.UserId);
            if (u == null) return;
            txtUserName.Text = u.DisplayName;
            txtEmail.Text = u.Email ?? "";
            txtContactNo.Text = u.Mobile ?? "";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = Validator.Clean(txtUserName.Text, 100), email = Validator.NormalizeEmail(txtEmail.Text);
            string contact = Validator.Clean(txtContactNo.Text, 20);
            string text = Validator.Clean(txtFeedback.Text, 2000), complaint = Validator.Clean(txtComplain.Text, 2000);
            int rating;

            if (name.Length == 0 || !Validator.IsEmail(email)) { Toast("Please enter your name and a valid e-mail address.", "danger"); return; }
            if (!int.TryParse(ddlRating.SelectedValue, out rating) || rating < 1 || rating > 5) { Toast("Please choose a rating.", "danger"); return; }
            if (text.Length == 0 && complaint.Length == 0) { Toast("Please write your feedback or suggestion.", "danger"); return; }
            if (!Throttle.Allow("feedback:" + ClientIp, 5, TimeSpan.FromHours(1))) { Toast("You have sent several messages recently. Please try again later.", "warning"); return; }

            try
            {
                Db.NonQuery(@"INSERT INTO dbo.Feedback(UserName, Email, ContactNo, Rating, FeedbackText, ComplaintOrSuggestion, FeedbackDate)
                              VALUES(@n, @e, @c, @r, @f, @s, GETDATE())",
                    Db.P("@n", name), Db.P("@e", email), Db.P("@c", contact), Db.P("@r", rating), Db.P("@f", text), Db.P("@s", complaint));
                txtFeedback.Text = string.Empty; txtComplain.Text = string.Empty; ddlRating.SelectedIndex = 0;
                Toast("Thank you! Your feedback has been received.", "success");
            }
            catch (Exception)
            {
                Toast("Sorry, we could not save your feedback right now. Please try again.", "danger");
            }
        }
    }
}
