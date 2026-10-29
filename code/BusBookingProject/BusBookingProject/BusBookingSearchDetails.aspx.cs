using System;
using System.Data;
using System.Globalization;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class BusBookingSearchDetails : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            string origin = Validator.Clean(Request.QueryString["from"], 50);
            string dest = Validator.Clean(Request.QueryString["to"], 50);
            DateTime date;
            if (origin.Length == 0 || dest.Length == 0
                || !DateTime.TryParseExact(Request.QueryString["date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Response.Redirect("Home.aspx", true);
                return;
            }

            litRoute.Text = Server.HtmlEncode(origin) + " <i class=\"fa-solid fa-arrow-right mx-1\"></i> " + Server.HtmlEncode(dest);
            litDate.Text = date.ToString("dddd, dd MMM yyyy");

            DataSet ds = Db.Proc("dbo.ispGetAvailableBusDetails",
                Db.P("@Origin", origin), Db.P("@Destination", dest), Db.P("@TravelDate", date.ToString("dd-MM-yyyy")));
            DataTable dt = ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();

            if (dt.Rows.Count == 0)
            {
                pnlEmpty.Visible = true;
                litEmpty.Text = "We have no departures from " + Server.HtmlEncode(origin) + " to " + Server.HtmlEncode(dest) + " on this date. Try another day.";
                litCount.Text = "0 buses";
                return;
            }
            litCount.Text = dt.Rows.Count + (dt.Rows.Count == 1 ? " bus" : " buses");
            rptBuses.DataSource = dt;
            rptBuses.DataBind();
        }

        protected string TypeKey(object type)
        {
            string s = (Convert.ToString(type) ?? "").ToUpperInvariant().Replace("-", " ");
            return s.Contains("NON") ? "nonac" : s.Contains("AC") ? "ac" : "other";
        }

        protected int SeatPercent(object available, object total)
        {
            int a = Convert.ToInt32(available), t = Convert.ToInt32(total);
            return t <= 0 ? 0 : Math.Max(0, Math.Min(100, a * 100 / t));
        }
    }
}
