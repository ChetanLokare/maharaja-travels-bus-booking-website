using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using BusBookingProject.Security;

namespace BusBookingProject.Services
{
    public class GatewayPayment
    {
        public string Id, OrderId, Status, Method, Currency;
        public long AmountPaise;
    }

    /// <summary>
    /// Razorpay Standard Checkout (cards, net banking, UPI, wallets). Card/UPI/bank details are entered on
    /// Razorpay's own pages and never touch this server, so no card data is stored or processed here.
    /// </summary>
    public static class RazorpayService
    {
        private const string Api = "https://api.razorpay.com/v1/";

        public static string KeyId { get { return Config.Get("Razorpay:KeyId"); } }
        private static string KeySecret { get { return Config.Get("Razorpay:KeySecret"); } }
        private static string WebhookSecret { get { return Config.Get("Razorpay:WebhookSecret"); } }

        public static bool IsConfigured { get { return KeyId.Length > 0 && KeySecret.Length > 0; } }

        public static string CreateOrder(long amountPaise, string receipt, out string error)
        {
            error = null;
            var body = new Dictionary<string, object>
            {
                { "amount", amountPaise }, { "currency", "INR" }, { "receipt", receipt },
                { "notes", new Dictionary<string, string> { { "pnr", receipt } } }
            };
            var json = Call("POST", "orders", new JavaScriptSerializer().Serialize(body), out error);
            if (json == null) return null;
            return json.ContainsKey("id") ? Convert.ToString(json["id"]) : null;
        }

        /// <summary>Checkout callback check: HMAC_SHA256(order_id|payment_id, key_secret) must equal the signature.</summary>
        public static bool VerifyCheckoutSignature(string orderId, string paymentId, string signature)
        {
            if (!IsConfigured || string.IsNullOrEmpty(signature)) return false;
            string expected = Crypto.HmacSha256Hex(KeySecret, orderId + "|" + paymentId);
            return Crypto.FixedTimeEquals(expected, signature.Trim().ToLowerInvariant());
        }

        public static bool VerifyWebhookSignature(string rawBody, string signature)
        {
            if (WebhookSecret.Length == 0 || string.IsNullOrEmpty(signature)) return false;
            return Crypto.FixedTimeEquals(Crypto.HmacSha256Hex(WebhookSecret, rawBody), signature.Trim().ToLowerInvariant());
        }

        /// <summary>Ask Razorpay directly - the browser's word is never enough.</summary>
        public static GatewayPayment FetchPayment(string paymentId, out string error)
        {
            var json = Call("GET", "payments/" + Uri.EscapeDataString(paymentId), null, out error);
            return json == null ? null : Map(json);
        }

        public static GatewayPayment Capture(string paymentId, long amountPaise, out string error)
        {
            var body = new Dictionary<string, object> { { "amount", amountPaise }, { "currency", "INR" } };
            var json = Call("POST", "payments/" + Uri.EscapeDataString(paymentId) + "/capture", new JavaScriptSerializer().Serialize(body), out error);
            return json == null ? null : Map(json);
        }

        public static string Refund(string paymentId, long amountPaise, out string error)
        {
            var body = new Dictionary<string, object> { { "amount", amountPaise } };
            var json = Call("POST", "payments/" + Uri.EscapeDataString(paymentId) + "/refund", new JavaScriptSerializer().Serialize(body), out error);
            return json != null && json.ContainsKey("id") ? Convert.ToString(json["id"]) : null;
        }

        private static GatewayPayment Map(Dictionary<string, object> j)
        {
            return new GatewayPayment
            {
                Id = Str(j, "id"), OrderId = Str(j, "order_id"), Status = Str(j, "status"),
                Method = Str(j, "method"), Currency = Str(j, "currency"),
                AmountPaise = j.ContainsKey("amount") && j["amount"] != null ? Convert.ToInt64(j["amount"]) : 0
            };
        }

        private static string Str(Dictionary<string, object> j, string key)
        {
            return j.ContainsKey(key) && j[key] != null ? Convert.ToString(j[key]) : string.Empty;
        }

        private static Dictionary<string, object> Call(string method, string path, string jsonBody, out string error)
        {
            error = null;
            if (!IsConfigured) { error = "The payment gateway is not configured."; return null; }
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(Api + path);
                req.Method = method;
                req.Timeout = 20000;
                req.Accept = "application/json";
                req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(KeyId + ":" + KeySecret));
                if (jsonBody != null)
                {
                    req.ContentType = "application/json";
                    byte[] data = Encoding.UTF8.GetBytes(jsonBody);
                    using (Stream s = req.GetRequestStream()) s.Write(data, 0, data.Length);
                }
                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var rd = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                {
                    return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(rd.ReadToEnd());
                }
            }
            catch (WebException ex)
            {
                string detail = "";
                try
                {
                    if (ex.Response != null)
                        using (var rd = new StreamReader(ex.Response.GetResponseStream())) detail = rd.ReadToEnd();
                }
                catch { }
                error = "The payment gateway rejected the request.";
                try { File.AppendAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/errors.log"), "[" + DateTime.Now + "] razorpay " + path + ": " + ex.Message + " " + detail + Environment.NewLine); } catch { }
                return null;
            }
        }
    }
}
