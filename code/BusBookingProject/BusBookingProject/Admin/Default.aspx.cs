using System;
using System.Web.UI;
using BusBookingProject.Services;

namespace BusBookingProject.Admin
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            litBuses.Text = Convert.ToString(Db.Scalar("SELECT COUNT(*) FROM dbo.BusMaster"));
            litSchedules.Text = Convert.ToString(Db.Scalar(
                "SELECT COUNT(*) FROM dbo.ScheduleMaster WHERE TRY_CONVERT(date, REPLACE([Date], '/', '-'), 105) >= CAST(GETDATE() AS date)"));
            litBookings.Text = Convert.ToString(Db.Scalar("SELECT COUNT(*) FROM dbo.PNRDetails WHERE ISNULL(Status,'Confirmed') = 'Confirmed'"));
            object rev = Db.Scalar("SELECT SUM(TotalAmount) FROM dbo.PNRDetails WHERE ISNULL(Status,'Confirmed') = 'Confirmed'");
            litRevenue.Text = (rev == null ? 0m : Convert.ToDecimal(rev)).ToString("N0");
            litRefunds.Text = Convert.ToString(Db.Scalar("SELECT COUNT(*) FROM dbo.Payments WHERE Status = 'RefundPending'"));
            litFailed.Text = Convert.ToString(Db.Scalar("SELECT COUNT(*) FROM dbo.AuditLog WHERE Event IN ('login_failed','admin_login_failed') AND CreatedAt > DATEADD(HOUR,-24,GETDATE())"));
        }
    }
}
