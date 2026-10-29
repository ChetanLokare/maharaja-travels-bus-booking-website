using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BusBookingProject.Admin
{
    public partial class BusScheduleReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGridView();
            }
        }
        private void BindGridView()
        {
            // Define connection string
            string connectionString = ConfigurationManager.ConnectionStrings["OnlineBusBookingConnectionString"].ConnectionString;

            // Define SQL query with JOIN to retrieve data from multiple tables
            string query = @"SELECT 
                    S.[ScheduleId],                    
                    B.[BusNo], 
                    B.[BusName], 
                    B.[BustType], 
                    R.[Origin], 
                    R.[Destination], 
                    S.[Date], 
                    B.[TotalSeat],  -- Corrected column name here
                    S.[AvailableSeats], 
                    S.[BookedSeats], 
                    S.[Fare], 
                    S.[EstimatedTime], 
                    S.[ArivalTime], 
                    S.[DepartureTime] 
                FROM 
                    [ScheduleMaster] AS S 
                INNER JOIN 
                    [BusMaster] AS B ON S.[BusId] = B.[BusId] 
                INNER JOIN 
                    [RouteDetails] AS R ON S.[RouteID] = R.[RouteID]";


            // Create DataTable to store data from SQL query
            DataTable dataTable = new DataTable();

            // Create SqlConnection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open connection
                connection.Open();

                // Execute SQL query and fill DataTable
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }
            }

            // Bind DataTable to GridView
            gdBusSchedule.DataSource = dataTable;
            gdBusSchedule.DataBind();
        }
        protected void gdBusSchedule_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check if the row is a data row
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the row number
                int rowIndex = e.Row.RowIndex + 1;

                // Find the Label control in the first cell of the row
                Label lblSrNo = (Label)e.Row.FindControl("lblSrNo");

                // Set the row number in the Label control
                if (lblSrNo != null)
                {
                    lblSrNo.Text = rowIndex.ToString();
                }
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {

        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Tell the compiler that the control is rendered
            // explicitly by overriding the VerifyRenderingInServerForm event.
        }

        [Obsolete]
        protected void BusReport_Click(object sender, EventArgs e)
        {
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=BusScheduleReport.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            // Create a new PDF document
            Document pdfdoc = new Document(new iTextSharp.text.Rectangle(PageSize.A4_LANDSCAPE.Rotate().Width + 100, PageSize.A4_LANDSCAPE.Height), 20f, 10f, 20f, 0f);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfdoc, Response.OutputStream);
            pdfdoc.Open();

            // Add Maharaja Travel's heading
            Paragraph heading = new Paragraph("Maharaja Travel's", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16));
            heading.Alignment = Element.ALIGN_CENTER;
            pdfdoc.Add(heading);

            // Add Booking Reports sub-heading
            Paragraph subHeading = new Paragraph("Bus Schedule Report", FontFactory.GetFont(FontFactory.HELVETICA, 14));
            subHeading.Alignment = Element.ALIGN_CENTER;
            pdfdoc.Add(subHeading);

            // Add current date
            Paragraph currentDate = new Paragraph(DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont(FontFactory.HELVETICA, 10));
            currentDate.Alignment = Element.ALIGN_RIGHT;
            pdfdoc.Add(currentDate);

            // Add line break
            pdfdoc.Add(new Chunk("\n"));

            // Render the GridView into the PDF
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gdBusSchedule.RenderControl(htw);
            StringReader sr = new StringReader(sw.ToString());
            HTMLWorker htmlparse = new HTMLWorker(pdfdoc);
            htmlparse.Parse(sr);

            pdfdoc.Close();
            Response.End();

            // Rebind the GridView
            gdBusSchedule.AllowPaging = true;
            gdBusSchedule.DataBind();
        }
    }
}