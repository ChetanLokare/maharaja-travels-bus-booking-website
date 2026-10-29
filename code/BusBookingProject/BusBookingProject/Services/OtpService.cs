using System;
using System.Data;
using System.Linq;
using BusBookingProject.Security;

namespace BusBookingProject.Services
{
    public class OtpResult
    {
        public bool Ok;
        public string Error;
        public int RetryAfterSeconds;
    }

    /// <summary>
    /// One-time codes: 6 digits, 10 minutes, 5 attempts, one use. Only an HMAC of the code is stored,
    /// so a database leak does not reveal live codes.
    /// </summary>
    public static class OtpService
    {
        public const string Register = "register";
        public const string Login = "login";
        public const string Reset = "reset";
        public const string Link = "link";

        private const int ExpiryMinutes = 10;
        private const int MaxAttempts = 5;
        private const int ResendSeconds = 60;
        private const int MaxPerHour = 5;

        private static string Pepper
        {
            get
            {
                string p = Config.Get("Security:OtpPepper");
                return p.Length > 0 ? p : "mbt-default-pepper-change-me";
            }
        }

        private static string Hash(string purpose, string target, string code)
        {
            return Crypto.HmacSha256Hex(Pepper, purpose + "|" + target.ToLowerInvariant() + "|" + code);
        }

        public static string PurposeLabel(string purpose)
        {
            switch (purpose)
            {
                case Register: return "verify your e-mail and finish creating your account";
                case Reset: return "reset your password";
                case Link: return "link your social account";
                default: return "sign in to your account";
            }
        }

        /// <param name="channel">"email" or "sms"</param>
        public static OtpResult Issue(string purpose, string target, string channel, string recipientName, string ip)
        {
            var res = new OtpResult();

            DataTable recent = Db.Query(
                @"SELECT TOP 1 DATEDIFF(SECOND, CreatedAt, GETDATE()) AS Age FROM dbo.OtpCodes
                  WHERE Purpose = @p AND Target = @t ORDER BY OtpId DESC", Db.P("@p", purpose), Db.P("@t", target));
            if (recent.Rows.Count > 0 && Convert.ToInt32(recent.Rows[0]["Age"]) < ResendSeconds)
            {
                res.RetryAfterSeconds = ResendSeconds - Convert.ToInt32(recent.Rows[0]["Age"]);
                res.Error = "Please wait " + res.RetryAfterSeconds + " seconds before requesting another code.";
                return res;
            }

            int lastHour = Convert.ToInt32(Db.Scalar(
                "SELECT COUNT(*) FROM dbo.OtpCodes WHERE Target = @t AND CreatedAt > DATEADD(HOUR, -1, GETDATE())", Db.P("@t", target)));
            if (lastHour >= MaxPerHour)
            {
                res.Error = "Too many codes requested. Please try again in an hour.";
                return res;
            }

            string code = Crypto.RandomDigits(6);

            Db.NonQuery("UPDATE dbo.OtpCodes SET ConsumedAt = GETDATE() WHERE Purpose = @p AND Target = @t AND ConsumedAt IS NULL",
                Db.P("@p", purpose), Db.P("@t", target));
            object idObj = Db.Scalar(
                @"INSERT INTO dbo.OtpCodes(Purpose, Target, CodeHash, ExpiresAt, RequestIp)
                  OUTPUT INSERTED.OtpId VALUES(@p, @t, @h, DATEADD(MINUTE, @m, GETDATE()), @ip)",
                Db.P("@p", purpose), Db.P("@t", target), Db.P("@h", Hash(purpose, target, code)), Db.P("@m", ExpiryMinutes), Db.P("@ip", ip));

            string err;
            bool sent;
            if (channel == "sms") sent = SmsService.SendOtp(target, code, out err);
            else sent = EmailService.SendOtp(target, recipientName, code, PurposeLabel(purpose), out err);

            if (!sent)
            {
                Db.NonQuery("DELETE FROM dbo.OtpCodes WHERE OtpId = @id", Db.P("@id", idObj));
                res.Error = err ?? "We could not deliver the code. Please try again.";
                return res;
            }
            res.Ok = true;
            return res;
        }

        public static bool Verify(string purpose, string target, string code, out string error)
        {
            error = null;
            code = new string((code ?? "").Where(char.IsDigit).ToArray());
            if (code.Length != 6) { error = "Enter the 6-digit code."; return false; }

            DataTable dt = Db.Query(
                @"SELECT TOP 1 OtpId, CodeHash, Attempts FROM dbo.OtpCodes
                  WHERE Purpose = @p AND Target = @t AND ConsumedAt IS NULL AND ExpiresAt > GETDATE() ORDER BY OtpId DESC",
                Db.P("@p", purpose), Db.P("@t", target));
            if (dt.Rows.Count == 0) { error = "This code has expired. Please request a new one."; return false; }

            long id = Convert.ToInt64(dt.Rows[0]["OtpId"]);
            int rows = Db.NonQuery("UPDATE dbo.OtpCodes SET Attempts = Attempts + 1 WHERE OtpId = @id AND Attempts < @max",
                Db.P("@id", id), Db.P("@max", MaxAttempts));
            if (rows == 0) { error = "Too many wrong attempts. Please request a new code."; return false; }

            if (!Crypto.FixedTimeEquals(Convert.ToString(dt.Rows[0]["CodeHash"]), Hash(purpose, target, code)))
            {
                error = "That code is not correct.";
                return false;
            }
            int used = Db.NonQuery("UPDATE dbo.OtpCodes SET ConsumedAt = GETDATE() WHERE OtpId = @id AND ConsumedAt IS NULL", Db.P("@id", id));
            if (used == 0) { error = "This code has already been used."; return false; }
            return true;
        }
    }
}
