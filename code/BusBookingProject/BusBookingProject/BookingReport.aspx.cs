using System;
using System.Data;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class BookingReport : BasePage
    {
        protected override bool RequiresLogin { get { return true; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            DataTable dt = BookingService.ListUserBookings(AuthService.UserId);
            pnlEmpty.Visible = dt.Rows.Count == 0;
            rptBookings.DataSource = dt;
            rptBookings.DataBind();
        }
    }
}
