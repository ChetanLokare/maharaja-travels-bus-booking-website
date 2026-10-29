using System;
using System.Web;

namespace BusBookingProject
{
    public partial class ErrorPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int code;
            int.TryParse(Request.QueryString["code"], out code);
            if (code == 404)
            {
                Response.StatusCode = 404;
                litCode.Text = "404";
                litTitle.Text = "We couldn't find that page";
                litMessage.Text = "The link may be old or mistyped.";
            }
            else
            {
                Response.StatusCode = 500;
                litCode.Text = "Oops";
                litTitle.Text = "Something went wrong on our side";
                litMessage.Text = "Your request could not be completed. Nothing has been charged without a confirmation. If this keeps happening, contact support.";
            }
            Response.TrySkipIisCustomErrors = true;
        }
    }
}
