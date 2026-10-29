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
    public partial class BookingReport : System.Web.UI.Page
    {
        #region Global Variable
        SqlConnection connString = new SqlConnection(ConfigurationManager.ConnectionStrings["OnlineBusBookingConnectionString"].ToString());
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] != null)
            {
                bingBookingReport();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        private void bingBookingReport()
        {
            DataSet dsGetData = new DataSet();
            SqlCommand sqlCmd = new SqlCommand();
            if (connString.State == ConnectionState.Closed)
            {
                connString.Open();
            }
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = "ispGetBookingReportByAdmin";
            sqlCmd.Connection = connString;
            SqlDataAdapter sda = new SqlDataAdapter(sqlCmd);
            sda.Fill(dsGetData);
            if (dsGetData.Tables[0].Rows.Count > 0)
            {
                gdTicketReport.DataSource = dsGetData.Tables[0];
                gdTicketReport.DataBind();
            }
            else
            {
                gdTicketReport.DataSource = null;
                gdTicketReport.EmptyDataText = "No Records Found";
                gdTicketReport.DataBind();
            }

        }

        [Obsolete]
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=BookingReport" + ".pdf");
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
            Paragraph subHeading = new Paragraph("Bus Booking Report", FontFactory.GetFont(FontFactory.HELVETICA, 14));
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
            gdTicketReport.RenderControl(htw);
            StringReader sr = new StringReader(sw.ToString());
            HTMLWorker htmlparse = new HTMLWorker(pdfdoc);
            htmlparse.Parse(sr);

            pdfdoc.Close();
            Response.End();

            // Rebind the GridView
            gdTicketReport.AllowPaging = true;
            gdTicketReport.DataBind();
        }





        public override void VerifyRenderingInServerForm(Control control)
        {
            // Tell the compiler that the control is rendered
            // explicitly by overriding the VerifyRenderingInServerForm event.
        }


    }
}