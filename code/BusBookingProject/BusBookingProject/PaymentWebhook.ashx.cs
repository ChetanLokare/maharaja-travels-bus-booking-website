using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using BusBookingProject.Services;

namespace BusBookingProject
{
    /// <summary>
    /// Razorpay server-to-server notification (configure in Dashboard > Webhooks: events payment.captured, payment.failed).
    /// It confirms the booking even if the customer closes the browser after paying. The body is only trusted
    /// after its HMAC signature has been verified with the webhook secret.
    /// </summary>
    public class PaymentWebhook : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase)) { context.Response.StatusCode = 405; return; }
            if (context.Request.ContentLength > 200000) { context.Response.StatusCode = 413; return; }

            string body;
            context.Request.InputStream.Position = 0;
            using (var rd = new StreamReader(context.Request.InputStream, Encoding.UTF8)) body = rd.ReadToEnd();

            if (!RazorpayService.VerifyWebhookSignature(body, context.Request.Headers["X-Razorpay-Signature"]))
            {
                context.Response.StatusCode = 400;
                return;
            }

            try
            {
                var root = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(body);
                string ev = Get(root, "event");
                var payload = root.ContainsKey("payload") ? root["payload"] as Dictionary<string, object> : null;
                var payment = payload != null && payload.ContainsKey("payment") ? payload["payment"] as Dictionary<string, object> : null;
                var entity = payment != null && payment.ContainsKey("entity") ? payment["entity"] as Dictionary<string, object> : null;
                if (entity == null) { context.Response.StatusCode = 200; return; }

                string paymentId = Get(entity, "id"), orderId = Get(entity, "order_id"), method = Get(entity, "method");
                long amount = entity.ContainsKey("amount") ? Convert.ToInt64(entity["amount"]) : 0;

                if (ev == "payment.failed")
                {
                    BookingService.MarkPayment(orderId, "Failed", paymentId, Get(entity, "error_description"), null);
                }
                else if (ev == "payment.captured" || ev == "order.paid")
                {
                    var dt = Db.Query("SELECT TOP 1 PNRNo, UserId, Amount FROM dbo.Payments WHERE GatewayOrderId = @o", Db.P("@o", orderId));
                    if (dt.Rows.Count == 1 && Convert.ToInt64(Math.Round(Convert.ToDecimal(dt.Rows[0]["Amount"]) * 100m)) == amount)
                    {
                        string pnr = Convert.ToString(dt.Rows[0]["PNRNo"]);
                        int userId = Convert.ToInt32(dt.Rows[0]["UserId"]);
                        int result = BookingService.Confirm(pnr, userId, orderId, paymentId, method, amount / 100m);
                        if (result == 1) EmailService.SendBookingConfirmed(pnr, userId);
                        else if (result == -3)
                        {
                            string err;
                            string refundId = RazorpayService.Refund(paymentId, amount, out err);
                            BookingService.MarkPayment(orderId, refundId != null ? "Refunded" : "RefundPending", paymentId, "seat no longer available", refundId);
                        }
                    }
                }
                context.Response.StatusCode = 200;
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;   // Razorpay retries on non-2xx
            }
        }

        private static string Get(Dictionary<string, object> d, string k)
        {
            return d != null && d.ContainsKey(k) && d[k] != null ? Convert.ToString(d[k]) : string.Empty;
        }
    }
}
