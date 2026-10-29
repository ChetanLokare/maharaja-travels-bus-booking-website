using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    public partial class SeatDetails : BasePage
    {
        protected int SeatRows, SeatCols, TotalSeats;
        protected string FareInvariant = "0", BookedCsv = "", HeldCsv = "", MineCsv = "";
        protected int MaxSeats { get { return BookingService.MaxSeats; } }
        protected int HoldMinutes { get { return BookingService.HoldMinutes; } }

        private ScheduleInfo schedule;
        private List<BoardingPoint> boarding;

        protected void Page_Load(object sender, EventArgs e)
        {
            int sid;
            if (!int.TryParse(Request.QueryString["sid"], NumberStyles.None, CultureInfo.InvariantCulture, out sid) || sid <= 0)
            {
                Response.Redirect("Home.aspx", true);
                return;
            }
            schedule = BookingService.GetSchedule(sid);
            if (schedule == null || !schedule.IsBookable)
            {
                Flash(schedule == null ? "That trip could not be found." : "Booking for this departure has closed.", "warning");
                Response.Redirect("Home.aspx", true);
                return;
            }

            SeatRows = schedule.SeatRow; SeatCols = schedule.SeatColumn; TotalSeats = schedule.TotalSeat;
            FareInvariant = schedule.Fare.ToString("0.##", CultureInfo.InvariantCulture);

            var booked = new List<int>(); var held = new List<int>(); var mine = new List<int>();
            BookingService.GetSeatStatus(sid, AuthService.UserId, booked, held, mine);
            BookedCsv = string.Join(",", booked); HeldCsv = string.Join(",", held); MineCsv = string.Join(",", mine);

            boarding = BookingService.GetBoardingPoints(schedule.BusId);

            if (IsPostBack) return;

            litRoute.Text = Server.HtmlEncode(schedule.Origin) + " <i class=\"fa-solid fa-arrow-right mx-1\"></i> " + Server.HtmlEncode(schedule.Destination);
            litMeta.Text = Server.HtmlEncode(schedule.BusName) + " (" + Server.HtmlEncode(schedule.BusType) + ") &middot; "
                + Server.HtmlEncode(schedule.TravelDate) + " &middot; departs " + Server.HtmlEncode(schedule.DepartureTime);

            ddlBoarding.Items.Add(new ListItem("Select boarding point", ""));
            foreach (var b in boarding)
                ddlBoarding.Items.Add(new ListItem(b.Place + " - " + b.Time, b.Id.ToString(CultureInfo.InvariantCulture)));
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            if (schedule == null) return;

            var seats = new List<int>();
            foreach (string part in (hdnSeats.Value ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int n;
                if (int.TryParse(part, NumberStyles.None, CultureInfo.InvariantCulture, out n)) seats.Add(n);
            }
            seats = seats.Distinct().OrderBy(x => x).ToList();

            string error = null;
            int boardingId = 0;
            if (seats.Count == 0) error = "Please select at least one seat.";
            else if (seats.Count > MaxSeats) error = "You can book at most " + MaxSeats + " seats at a time.";
            else if (seats.Any(s => s < 1 || s > schedule.TotalSeat)) error = "One of the selected seats is not valid.";
            else if (!int.TryParse(ddlBoarding.SelectedValue, NumberStyles.None, CultureInfo.InvariantCulture, out boardingId) || !boarding.Any(b => b.Id == boardingId))
                error = "Please choose a boarding point.";

            if (error == null)
            {
                // quick check against the live seat map (the real guarantee is the atomic hold on the next step)
                var unavailable = seats.Where(s => BookedCsv.Split(',').Contains(s.ToString()) || HeldCsv.Split(',').Contains(s.ToString())).ToList();
                if (unavailable.Count > 0) error = "Seat " + string.Join(", ", unavailable) + " was just taken. Please pick another.";
            }

            if (error != null)
            {
                Toast(error, "danger");
                return;
            }

            Session["Checkout"] = new CheckoutState { ScheduleId = schedule.ScheduleId, BoardingId = boardingId, Seats = seats };
            Response.Redirect("PassengerDetailsInfo.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
