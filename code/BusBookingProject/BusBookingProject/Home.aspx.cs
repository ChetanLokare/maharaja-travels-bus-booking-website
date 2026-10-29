using System;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class Home : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            DataTable cities = Db.Query("SELECT DISTINCT LTRIM(RTRIM(CityName)) AS CityName FROM dbo.CityDetails WHERE CityName IS NOT NULL ORDER BY 1");
            foreach (var ddl in new[] { ddlOrigin, ddlDestination })
            {
                ddl.DataSource = cities;
                ddl.DataTextField = "CityName";
                ddl.DataValueField = "CityName";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("Select city", ""));
            }

            DateTime today = DateTime.Today;
            txtJourneyDate.Text = today.ToString("yyyy-MM-dd");
            txtJourneyDate.Attributes["min"] = today.ToString("yyyy-MM-dd");
            txtJourneyDate.Attributes["max"] = today.AddDays(90).ToString("yyyy-MM-dd");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string origin = Validator.Clean(ddlOrigin.SelectedValue, 50);
            string dest = Validator.Clean(ddlDestination.SelectedValue, 50);
            DateTime date;

            string error = null;
            if (origin.Length == 0 || dest.Length == 0) error = "Please choose both the origin and the destination.";
            else if (string.Equals(origin, dest, StringComparison.OrdinalIgnoreCase)) error = "Origin and destination must be different.";
            else if (!DateTime.TryParseExact(txtJourneyDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) error = "Please choose a valid journey date.";
            else if (date < DateTime.Today) error = "The journey date cannot be in the past.";
            else if (date > DateTime.Today.AddDays(90)) error = "You can book up to 90 days in advance.";

            if (error != null)
            {
                litError.Text = Server.HtmlEncode(error);
                pnlError.Visible = true;
                return;
            }

            Response.Redirect("BusBookingSearchDetails.aspx?from=" + Server.UrlEncode(origin) + "&to=" + Server.UrlEncode(dest)
                + "&date=" + txtJourneyDate.Text, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
