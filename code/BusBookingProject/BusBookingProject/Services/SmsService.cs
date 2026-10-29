using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Hosting;
using BusBookingProject.Security;

namespace BusBookingProject.Services
{
    /// <summary>SMS delivery. Provider "Twilio" is built in; empty provider = disabled (callers fall back to e-mail).</summary>
    public static class SmsService
    {
        public static bool IsEnabled
        {
            get { return string.Equals(Config.Get("Sms:Provider"), "Twilio", StringComparison.OrdinalIgnoreCase) && Config.Get("Sms:Twilio:AccountSid").Length > 0; }
        }

        public static bool SendOtp(string mobile10, string code, out string error)
        {
            error = null;
            string body = "Your Maharaja Travel's verification code is " + code + ". It expires in 10 minutes. Do not share it with anyone.";
            if (!IsEnabled)
            {
                error = "SMS is not configured.";
                return false;
            }
            try
            {
                string sid = Config.Get("Sms:Twilio:AccountSid");
                string token = Config.Get("Sms:Twilio:AuthToken");
                string from = Config.Get("Sms:Twilio:From");
                var req = (HttpWebRequest)WebRequest.Create("https://api.twilio.com/2010-04-01/Accounts/" + Uri.EscapeDataString(sid) + "/Messages.json");
                req.Method = "POST";
                req.Timeout = 15000;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(sid + ":" + token));
                byte[] data = Encoding.UTF8.GetBytes("To=" + Uri.EscapeDataString("+91" + mobile10) + "&From=" + Uri.EscapeDataString(from) + "&Body=" + Uri.EscapeDataString(body));
                using (Stream s = req.GetRequestStream()) s.Write(data, 0, data.Length);
                using (var resp = (HttpWebResponse)req.GetResponse()) { return (int)resp.StatusCode < 300; }
            }
            catch (WebException)
            {
                error = "We could not send the SMS right now.";
                return false;
            }
        }
    }
}
