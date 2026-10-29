using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;

namespace BusBookingProject.Admin
{
    public partial class BusDetailsReport : System.Web.UI.Page
    {
        #region Global Variable
        SqlConnection connString = new SqlConnection(ConfigurationManager.ConnectionStrings["OnlineBusBookingConnectionString"].ToString());
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserName"] != null)
                {
                    bindBusDetailsReport();
                }
                else
                {
                    Response.Redirect("AdminLogin.aspx");
                }
            }
        }

        private void bindBusDetailsReport()
        {
            DataSet dsGetData = new DataSet();
            SqlCommand sqlCmd = new SqlCommand();
            if (connString.State == ConnectionState.Closed)
            {
                connString.Open();
            }
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = "ispGetBusDetailsForUpdation";
            sqlCmd.Connection = connString;
            SqlDataAdapter sda = new SqlDataAdapter(sqlCmd);
            sda.Fill(dsGetData);
            if (dsGetData.Tables[0].Rows.Count > 0)
            {
                gdBusDetails.DataSource = dsGetData.Tables[0];
                gdBusDetails.DataBind();
            }
            else
            {
                gdBusDetails.DataSource = null;
                gdBusDetails.EmptyDataText = "No Records Found";
                gdBusDetails.DataBind();
            }
        }

        protected void gdBusDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink klnikUpdate = (HyperLink)e.Row.FindControl("hlinkUpdate");
                HiddenField hdnBusID = (HiddenField)e.Row.FindControl("hdnPNRNo");
                HyperLink hlinkSchedule = (HyperLink)e.Row.FindControl("hlinkAddSchedule");
                HiddenField hdnRouteID = (HiddenField)e.Row.FindControl("hdnRouteID");
                klnikUpdate.NavigateUrl = "BusDetails.aspx?BusID=" + hdnBusID.Value;
                hlinkSchedule.NavigateUrl = "BusScheduleDetails.aspx?BusID=" + hdnBusID.Value + "&RouteID=" + hdnRouteID.Value;
            }
        }

        [Obsolete]
        protected void BusReport_Click(object sender, EventArgs e)
        {
            // Bind the GridView data
            bindBusDetailsReport();

            // Create the PDF document
            Document pdfdoc = new Document(new iTextSharp.text.Rectangle(PageSize.A4_LANDSCAPE.Rotate().Width + 100, PageSize.A4_LANDSCAPE.Height), 20f, 10f, 20f, 0f);
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(pdfdoc, memoryStream);
            pdfdoc.Open();

            // Add Maharaja Travel's heading
            Paragraph heading = new Paragraph("Maharaja Travel's", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16));
            heading.Alignment = Element.ALIGN_CENTER;
            pdfdoc.Add(heading);

            // Add Booking Reports sub-heading
            Paragraph subHeading = new Paragraph("Bus Details Report", FontFactory.GetFont(FontFactory.HELVETICA, 14));
            subHeading.Alignment = Element.ALIGN_CENTER;
            pdfdoc.Add(subHeading);

            // Add current date
            Paragraph currentDate = new Paragraph(DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont(FontFactory.HELVETICA, 10));
            currentDate.Alignment = Element.ALIGN_RIGHT;
            pdfdoc.Add(currentDate);

            // Add line break
            pdfdoc.Add(new Chunk("\n"));

            // Create a table with the same number of columns as labels
            PdfPTable table = new PdfPTable(9); // Adjust the number of columns based on the number of labels

            // Add headers to the table
            table.AddCell("Sr.No");
            table.AddCell("Bus ID");
            table.AddCell("Bus No");
            table.AddCell("Bus Name");
            table.AddCell("Bus Type");
            table.AddCell("Seat Columns");
            table.AddCell("Seat Row");
            table.AddCell("Origin");
            table.AddCell("Destination");

            // Add data rows to the table
            int serialNumber = 1;
            foreach (GridViewRow row in gdBusDetails.Rows)
            {
                table.AddCell(serialNumber.ToString());
                table.AddCell(((Label)row.FindControl("lblBusID")).Text);
                table.AddCell(((Label)row.FindControl("lblBusNo")).Text);
                table.AddCell(((Label)row.FindControl("lblBusName")).Text);
                table.AddCell(((Label)row.FindControl("lblBusType")).Text);
                table.AddCell(((Label)row.FindControl("lblSeatCol")).Text);
                table.AddCell(((Label)row.FindControl("lblSeatRow")).Text);
                table.AddCell(((Label)row.FindControl("lblOrigin")).Text);
                table.AddCell(((Label)row.FindControl("lblDestination")).Text);

                serialNumber++;
            }

            // Add the table to the PDF document
            pdfdoc.Add(table);

            pdfdoc.Close();

            // Clear the response
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment;filename=BusReport.pdf");
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Tell the compiler that the control is rendered
            // explicitly by overriding the VerifyRenderingInServerForm event.
        }


    }
}