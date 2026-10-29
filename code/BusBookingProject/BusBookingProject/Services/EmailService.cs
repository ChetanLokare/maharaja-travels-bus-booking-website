using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Hosting;
using BusBookingProject.Security;

namespace BusBookingProject.Services
{
    public class MailAttachment
    {
        public string FileName;
        public byte[] Content;
    }

    /// <summary>All outgoing e-mail: OTP, sign-in alerts, welcome, ticket and receipt.</summary>
    public static class EmailService
    {
        private static readonly object LogLock = new object();

        public static bool IsConfigured
        {
            get { return Config.Get("Smtp:Host").Length > 0 && Config.Get("Smtp:From").Length > 0; }
        }

        /// <summary>On a developer PC without SMTP the message is written to App_Data\dev-outbox.log instead of being sent.</summary>
        private static bool DevOutbox
        {
            get { return !IsConfigured && HttpContext.Current != null && HttpContext.Current.Request.IsLocal; }
        }

        public static bool CanDeliver { get { return IsConfigured || DevOutbox; } }

        /// <summary>True only on localhost while SMTP is not configured (developer convenience).</summary>
        public static bool IsDevMode { get { return DevOutbox; } }

        // ------------------------------------------------------------------ core
        public static bool Send(string to, string subject, string html, string plain, IList<MailAttachment> attachments, out string error)
        {
            error = null;
            if (!Validator.IsEmail(to)) { error = "Invalid e-mail address."; return false; }

            if (!IsConfigured)
            {
                if (!(HttpContext.Current != null && HttpContext.Current.Request.IsLocal)) { error = "E-mail is not configured on the server."; return false; }
                if (!WriteDevOutbox(to, subject, plain, attachments))
                {
                    error = "Could not write App_Data\\dev-outbox.log - check folder permissions (see App_Data\\errors.log).";
                    return false;
                }
                return true;
            }
            return SendSmtp(to, subject, html, plain, attachments, out error);
        }

        private static bool SendSmtp(string to, string subject, string html, string plain, IList<MailAttachment> attachments, out string error)
        {
            error = null;
            try
            {
                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(Config.Get("Smtp:From"), Config.Get("Smtp:FromName", "Maharaja Travel's"));
                    msg.To.Add(new MailAddress(to));
                    msg.Subject = subject.Replace("\r", " ").Replace("\n", " ");
                    msg.SubjectEncoding = Encoding.UTF8;
                    msg.BodyEncoding = Encoding.UTF8;
                    msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plain, Encoding.UTF8, MediaTypeNames.Text.Plain));
                    msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html));
                    if (attachments != null)
                    {
                        foreach (var a in attachments)
                            msg.Attachments.Add(new Attachment(new MemoryStream(a.Content), a.FileName, "application/pdf"));
                    }
                    using (var smtp = new SmtpClient(Config.Get("Smtp:Host"), Config.GetInt("Smtp:Port", 587)))
                    {
                        smtp.EnableSsl = Config.GetBool("Smtp:EnableSsl", true);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Timeout = 20000;
                        if (Config.Get("Smtp:User").Length > 0)
                            smtp.Credentials = new NetworkCredential(Config.Get("Smtp:User"), Config.Get("Smtp:Password"));
                        smtp.Send(msg);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                error = "We could not send the e-mail right now.";
                try { File.AppendAllText(HostingEnvironment.MapPath("~/App_Data/errors.log"), "[" + DateTime.Now + "] smtp: " + ex.GetBaseException().Message + Environment.NewLine); } catch { }
                return false;
            }
        }

        private static bool WriteDevOutbox(string to, string subject, string plain, IList<MailAttachment> attachments)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (DEV OUTBOX - SMTP not configured) ===");
            sb.AppendLine("To: " + to);
            sb.AppendLine("Subject: " + subject);
            if (attachments != null && attachments.Count > 0)
            {
                var names = new List<string>();
                foreach (var a in attachments) names.Add(a.FileName + " (" + a.Content.Length + " bytes)");
                sb.AppendLine("Attachments: " + string.Join(", ", names));
            }
            sb.AppendLine();
            sb.AppendLine(plain);
            sb.AppendLine();

            // Visible in Visual Studio: View > Output > "Debug" (only while debugging; compiled out of Release builds)
            System.Diagnostics.Debug.WriteLine(sb.ToString());

            try
            {
                string dir = HostingEnvironment.MapPath("~/App_Data");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                lock (LogLock) File.AppendAllText(Path.Combine(dir, "dev-outbox.log"), sb.ToString());
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("dev-outbox write failed: " + ex.Message);
                try { File.AppendAllText(Path.Combine(HostingEnvironment.MapPath("~/App_Data"), "errors.log"), "[" + DateTime.Now + "] dev-outbox: " + ex.Message + Environment.NewLine); } catch { }
                return false;
            }
        }

        /// <summary>Send after the response has been produced so the user is not kept waiting.</summary>
        public static void SendInBackground(Func<MailAttachment[]> attachmentFactory, string to, string subject, string html, string plain)
        {
            bool dev = DevOutbox;
            bool configured = IsConfigured;
            if (!configured && !dev) return;
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                try
                {
                    MailAttachment[] atts = attachmentFactory != null ? attachmentFactory() : null;
                    string err;
                    if (configured) SendSmtp(to, subject, html, plain, atts, out err);
                    else WriteDevOutbox(to, subject, plain, atts);
                }
                catch { }
            });
        }

        // ------------------------------------------------------------------ templates
        private static string H(string s) { return HttpUtility.HtmlEncode(s ?? string.Empty); }

        private static string Layout(string title, string bodyHtml)
        {
            return "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"></head>" +
                "<body style=\"margin:0;padding:0;background:#eef0f7;font-family:Segoe UI,Roboto,Arial,sans-serif;color:#1f2937\">" +
                "<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"background:#eef0f7;padding:24px 12px\"><tr><td align=\"center\">" +
                "<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden\">" +
                "<tr><td style=\"background:linear-gradient(135deg,#312e81,#6d28d9);padding:22px 28px;color:#fff;font-size:20px;font-weight:700\">" +
                "&#128652; Maharaja Travel&#39;s</td></tr>" +
                "<tr><td style=\"padding:28px\"><h1 style=\"margin:0 0 14px;font-size:20px;color:#111827\">" + H(title) + "</h1>" + bodyHtml + "</td></tr>" +
                "<tr><td style=\"padding:16px 28px;background:#f8f9fc;color:#6b7280;font-size:12px\">This is an automated message from Maharaja Travel&#39;s. " +
                "Need help? Write to " + H(Config.Get("App:SupportEmail")) + ".</td></tr></table></td></tr></table></body></html>";
        }

        private static string Button(string url, string text)
        {
            return "<p style=\"margin:22px 0\"><a href=\"" + H(url) + "\" style=\"background:#4338ca;color:#fff;text-decoration:none;padding:12px 22px;border-radius:8px;font-weight:600;display:inline-block\">" + H(text) + "</a></p>";
        }

        public static bool SendOtp(string to, string name, string code, string purposeLabel, out string error)
        {
            string html = Layout("Your verification code",
                "<p>Hi " + H(name) + ",</p><p>Use this code to " + H(purposeLabel) + ":</p>" +
                "<p style=\"font-size:34px;letter-spacing:8px;font-weight:700;color:#4338ca;margin:18px 0\">" + H(code) + "</p>" +
                "<p>It expires in 10 minutes and works only once. <strong>Never share it with anyone</strong> - our team will never ask for it.</p>" +
                "<p style=\"color:#6b7280\">If you did not request this, you can safely ignore this e-mail.</p>");
            string plain = "Hi " + name + ",\r\n\r\nYour Maharaja Travel's code to " + purposeLabel + " is: " + code + "\r\nIt expires in 10 minutes. Never share it.\r\n";
            bool sent = Send(to, "Your Maharaja Travel's verification code: " + code, html, plain, null, out error);
            // developer mode only: let the code page show the code (never when SMTP is configured or off-localhost)
            if (sent && DevOutbox && HttpContext.Current != null && HttpContext.Current.Session != null)
                HttpContext.Current.Session["DevOtp"] = code;
            return sent;
        }

        public static void SendLoginAlert(string to, string name, string method, string ip, string userAgent)
        {
            string when = DateTime.Now.ToString("dddd, dd MMM yyyy 'at' hh:mm tt");
            string html = Layout("New sign-in to your account",
                "<p>Hi " + H(name) + ",</p><p>You just signed in to your Maharaja Travel&#39;s account.</p>" +
                "<table style=\"font-size:14px;border-collapse:collapse\">" +
                "<tr><td style=\"padding:4px 14px 4px 0;color:#6b7280\">When</td><td>" + H(when) + "</td></tr>" +
                "<tr><td style=\"padding:4px 14px 4px 0;color:#6b7280\">Method</td><td>" + H(method) + "</td></tr>" +
                "<tr><td style=\"padding:4px 14px 4px 0;color:#6b7280\">IP address</td><td>" + H(ip) + "</td></tr>" +
                "<tr><td style=\"padding:4px 14px 4px 0;color:#6b7280\">Device</td><td>" + H(Shorten(userAgent, 90)) + "</td></tr></table>" +
                "<p>If this was you, no action is needed. If not, reset your password right away.</p>" +
                Button(Config.BaseUrl + "/ForgotPassword.aspx", "Secure my account"));
            string plain = "Hi " + name + ",\r\nYou signed in on " + when + " using " + method + " from IP " + ip + ".\r\nIf this was not you, reset your password: " + Config.BaseUrl + "/ForgotPassword.aspx\r\n";
            SendInBackground(null, to, "New sign-in to your Maharaja Travel's account", html, plain);
        }

        public static void SendWelcome(string to, string name)
        {
            string html = Layout("Welcome aboard!",
                "<p>Hi " + H(name) + ",</p><p>Your e-mail is verified and your account is ready. Search a route, pick your seats and pay securely by UPI, card or net banking.</p>" +
                Button(Config.BaseUrl + "/Home.aspx", "Book your first trip"));
            SendInBackground(null, to, "Welcome to Maharaja Travel's", html, "Hi " + name + ", your account is ready. Book at " + Config.BaseUrl + "/Home.aspx");
        }

        public static void SendPasswordChanged(string to, string name)
        {
            string html = Layout("Your password was changed",
                "<p>Hi " + H(name) + ",</p><p>The password on your account was just changed. If you did this, you are all set.</p>" +
                "<p><strong>If you did not,</strong> reset it immediately and contact support.</p>" +
                Button(Config.BaseUrl + "/ForgotPassword.aspx", "Reset password"));
            SendInBackground(null, to, "Your Maharaja Travel's password was changed", html, "Your password was changed. If this was not you, reset it at " + Config.BaseUrl + "/ForgotPassword.aspx");
        }

        public static void SendBookingConfirmed(string pnr, int userId)
        {
            BookingDetail b = BookingService.GetBooking(pnr, userId);
            if (b == null || string.IsNullOrEmpty(b.ContactEmail)) return;

            string route = b.Origin + " to " + b.Destination;
            var rows = new StringBuilder();
            foreach (var p in b.Passengers)
                rows.Append("<tr><td style=\"padding:4px 12px 4px 0\">" + H(p.FullName) + "</td><td style=\"padding:4px 12px\">Seat " + H(p.SeatNo) + "</td></tr>");

            string html = Layout("Booking confirmed - PNR " + b.Pnr,
                "<p>Your payment was received and your seats are confirmed. Your e-ticket and payment receipt are attached as PDF files.</p>" +
                "<table style=\"font-size:14px;border-collapse:collapse;margin:10px 0\">" +
                "<tr><td style=\"padding:4px 14px 4px 0;color:#6b7280\">PNR</td><td><strong>" + H(b.Pnr) + "</strong></td></tr>" +
                "<tr><td style=\"padding:4px 14px 4px 0;color:#6b7280\">Route</td><td>" + H(route) + "</td></tr>" +
                "<tr><td style=\"padding:4px 14px 4px 0;color:#6b7280\">Date</td><td>" + H(b.TravelDate) + ", departs " + H(b.DepartureTime) + "</td></tr>" +
                "<tr><td style=\"padding:4px 14px 4px 0;color:#6b7280\">Bus</td><td>" + H(b.BusName) + " (" + H(b.BusNo) + ")</td></tr>" +
                "<tr><td style=\"padding:4px 14px 4px 0;color:#6b7280\">Boarding</td><td>" + H(b.BoardingPlace) + " at " + H(b.BoardingTime) + "</td></tr>" +
                "<tr><td style=\"padding:4px 14px 4px 0;color:#6b7280\">Amount paid</td><td><strong>Rs. " + b.TotalAmount.ToString("N2") + "</strong></td></tr></table>" +
                "<p style=\"margin-bottom:4px\"><strong>Passengers</strong></p><table style=\"font-size:14px\">" + rows + "</table>" +
                "<p style=\"color:#6b7280\">Please carry a valid photo ID and show the QR code on your ticket when boarding.</p>" +
                Button(Config.BaseUrl + "/BookingReport.aspx", "View my bookings"));
            string plain = "Booking confirmed. PNR " + b.Pnr + "\r\n" + route + "\r\n" + b.TravelDate + " " + b.DepartureTime + "\r\nAmount paid: Rs. " + b.TotalAmount.ToString("N2") + "\r\nTicket and receipt are attached.";

            string to = b.ContactEmail;
            SendInBackground(() => new[]
            {
                new MailAttachment { FileName = "Ticket-" + b.Pnr + ".pdf", Content = PdfService.BuildTicket(b) },
                new MailAttachment { FileName = "Receipt-" + b.Pnr + ".pdf", Content = PdfService.BuildReceipt(b) }
            }, to, "Your ticket - PNR " + b.Pnr + " (" + route + ")", html, plain);
        }

        public static void SendPaymentIssue(string to, string pnr, string message)
        {
            string html = Layout("Action needed on your payment",
                "<p>" + H(message) + "</p><p>Booking reference: <strong>" + H(pnr) + "</strong>. If money was deducted it will be refunded to your original payment method, normally within 5-7 working days.</p>");
            SendInBackground(null, to, "Your Maharaja Travel's payment - " + pnr, html, message + " Reference: " + pnr);
        }

        private static string Shorten(string s, int n)
        {
            s = s ?? string.Empty;
            return s.Length <= n ? s : s.Substring(0, n) + "...";
        }
    }
}
