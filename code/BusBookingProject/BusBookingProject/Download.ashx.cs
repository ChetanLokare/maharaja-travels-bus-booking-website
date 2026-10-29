using System;
using System.Web;
using System.Web.SessionState;
using BusBookingProject.Security;
using BusBookingProject.Services;

namespace BusBookingProject
{
    /// <summary>Ticket / receipt PDF. Only the account that made the booking can download it.</summary>
    public class Download : IHttpHandler, IReadOnlySessionState
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            if (!AuthService.IsUser) { context.Response.StatusCode = 401; return; }

            string pnr = (context.Request.QueryString["pnr"] ?? "").ToUpperInvariant();
            string type = (context.Request.QueryString["type"] ?? "").ToLowerInvariant();
            if (!Validator.IsPnr(pnr) || (type != "ticket" && type != "receipt")) { context.Response.StatusCode = 400; return; }

            BookingDetail b = BookingService.GetBooking(pnr, AuthService.UserId);
            if (b == null || b.Status != "Confirmed") { context.Response.StatusCode = 404; return; }

            byte[] pdf = type == "ticket" ? PdfService.BuildTicket(b) : PdfService.BuildReceipt(b);
            string name = (type == "ticket" ? "Ticket-" : "Receipt-") + b.Pnr + ".pdf";

            context.Response.Clear();
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + name + "\"");
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.BinaryWrite(pdf);
            context.Response.End();
        }
    }
}
