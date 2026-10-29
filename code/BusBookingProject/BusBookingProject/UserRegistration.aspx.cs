using System;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class UserRegistration : BasePage
    {
        protected bool GoogleEnabled { get { return OAuthService.IsConfigured(OAuthService.Google); } }
        protected bool MicrosoftEnabled { get { return OAuthService.IsConfigured(OAuthService.Microsoft); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && AuthService.IsUser) Response.Redirect("Home.aspx", true);
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string first = Validator.Clean(txtFirstName.Text, 50), last = Validator.Clean(txtLastName.Text, 50);
            string email = Validator.NormalizeEmail(txtEmail.Text);
            string mobile = Validator.NormalizeMobile(txtMobile.Text);
            string address = Validator.Clean(txtAddress.Text, 200), city = Validator.Clean(txtCity.Text, 50), pin = Validator.Clean(txtPincode.Text, 6);
            string password = txtPassword.Text ?? "", confirm = txtConfirm.Text ?? "";

            string error = null;
            if (!Validator.IsName(first) || !Validator.IsName(last)) error = "Please enter your first and last name (letters only).";
            else if (!Validator.IsEmail(email)) error = "Please enter a valid e-mail address.";
            else if (mobile == null) error = "Please enter a valid 10-digit Indian mobile number.";
            else if (pin.Length > 0 && !Validator.IsPincode(pin)) error = "Pincode must be 6 digits.";
            else if (password != confirm) error = "The two passwords do not match.";
            else if (!chkTerms.Checked) error = "Please accept the terms to continue.";
            else error = PasswordHasher.CheckPolicy(password, email);

            if (error != null) { Toast(error, "danger"); return; }

            if (!Throttle.Allow("reg-ip:" + ClientIp, 8, TimeSpan.FromHours(1)))
            {
                Toast("Too many sign-up attempts from this network. Please try again later.", "danger");
                return;
            }
            if (UserRepository.EmailOrMobileExists(email, mobile))
            {
                Toast("An account with this e-mail or mobile number already exists. Try signing in or resetting your password.", "warning");
                return;
            }

            OtpResult r = OtpService.Issue(OtpService.Register, email, "email", first, ClientIp);
            if (!r.Ok) { Toast(r.Error, "danger"); return; }

            // nothing is written to the users table until the e-mail address is proven
            Session["Otp"] = new OtpContext
            {
                Purpose = OtpService.Register, Target = email, Channel = "email", Method = "New account", FirstName = first, LastName = last,
                Email = email, Mobile = mobile, Address = address.Length == 0 ? null : address, City = city.Length == 0 ? null : city,
                Pincode = pin.Length == 0 ? null : pin, PasswordHash = PasswordHasher.Hash(password)
            };
            Response.Redirect("OtpVerify.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
