using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BusBookingProject.Admin
{
    public partial class BusScheduleDetails : System.Web.UI.Page
    {
        #region Global Variable
        SqlConnection connString = new SqlConnection(ConfigurationManager.ConnectionStrings["OnlineBusBookingConnectionString"].ToString());
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (!IsPostBack)
            {
                if (Session["UserName"] != null)
                {

                }
                else
                {
                    Response.Redirect("AdminLogin.aspx");
                }
            }
        }

        private int addBusScheduleData()
        {
            int ResultCout = 0;
            SqlCommand sqlCmd = new SqlCommand();
            if (connString.State == ConnectionState.Closed)
            {
                connString.Open();
            }
            sqlCmd.CommandType = CommandType.StoredProcedure;
            string formattedDate = DateTime.ParseExact(txtDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
            // Convert and format arrival time
            DateTime arrivalTime;
            if (DateTime.TryParse(txtArrival.Text, out arrivalTime))
            {
                string formattedArrivalTime = arrivalTime.ToString("hh:mm tt");

                // Convert and format departure time
                DateTime departureTime;
                if (DateTime.TryParse(txtDeparture.Text, out departureTime))
                {
                    string formattedDepartureTime = departureTime.ToString("hh:mm tt");

                    sqlCmd.Parameters.AddWithValue("@Date", formattedDate);
                    sqlCmd.Parameters.AddWithValue("@BusID", Convert.ToInt32(Request.QueryString["BusID"]));
                    sqlCmd.Parameters.AddWithValue("@Fare", Convert.ToDecimal(txtFare.Text));
                    sqlCmd.Parameters.AddWithValue("@EstimatdTime", Convert.ToString(txtTravelTime.Text));
                    sqlCmd.Parameters.AddWithValue("@ArrivalTime", formattedArrivalTime);
                    sqlCmd.Parameters.AddWithValue("@DepartureTime", formattedDepartureTime);
                    sqlCmd.Parameters.AddWithValue("@RouteID", Convert.ToInt32(Request.QueryString["RouteID"]));
                    sqlCmd.CommandText = "ispAddBusSchedule";
                    sqlCmd.Connection = connString;
                    ResultCout = sqlCmd.ExecuteNonQuery();
                }
                else
                {
                    // Handle invalid departure time
                    // Display error message or perform other actions
                }
            }
            else
            {
                // Handle invalid arrival time
                // Display error message or perform other actions
            }

            return ResultCout;

        }
        protected void btnSaveSchedule_Click(object sender, EventArgs e)
        {
            int result = addBusScheduleData();
            if (result == -1)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Bus Schedule Details has been added successfully')", true);
                txtDeparture.Text = "";
                txtDate.Text = "";
                txtArrival.Text = "";
                txtFare.Text = "";
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Error occur please contact your system administrator')", true);
            }
        }
    }
}
 