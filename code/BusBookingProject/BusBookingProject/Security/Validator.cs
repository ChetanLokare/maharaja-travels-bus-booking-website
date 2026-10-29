using System;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace BusBookingProject.Security
{
    /// <summary>Server-side input validation / normalisation. Client-side checks are only a convenience.</summary>
    public static class Validator
    {
        private static readonly Regex NameRx = new Regex(@"^[\p{L}][\p{L}\p{M} .'\-]{0,49}$", RegexOptions.Compiled);
        private static readonly Regex PinRx = new Regex(@"^[1-9][0-9]{5}$", RegexOptions.Compiled);
        private static readonly Regex PnrRx = new Regex(@"^[A-Z0-9]{6,12}$", RegexOptions.Compiled);

        /// <summary>Trim, drop control characters and cap the length.</summary>
        public static string Clean(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var sb = new StringBuilder(value.Length);
            foreach (char c in value.Trim())
            {
                if (!char.IsControl(c) || c == ' ') sb.Append(c);
            }
            string s = sb.ToString().Trim();
            return s.Length > maxLength ? s.Substring(0, maxLength) : s;
        }

        public static bool IsEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Length > 150) return false;
            if (email.IndexOfAny(new[] { ' ', ',', ';', '<', '>', '\r', '\n' }) >= 0) return false;
            try
            {
                var m = new MailAddress(email);
                return string.Equals(m.Address, email, StringComparison.OrdinalIgnoreCase) && m.Host.Contains(".");
            }
            catch (FormatException) { return false; }
        }

        public static string NormalizeEmail(string email)
        {
            return (email ?? string.Empty).Trim().ToLowerInvariant();
        }

        /// <summary>Indian mobile number -> 10 digits starting with 6-9, or null.</summary>
        public static string NormalizeMobile(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            var digits = new StringBuilder();
            foreach (char c in input) if (c >= '0' && c <= '9') digits.Append(c);
            string d = digits.ToString();
            if (d.Length == 12 && d.StartsWith("91")) d = d.Substring(2);
            else if (d.Length == 11 && d.StartsWith("0")) d = d.Substring(1);
            return d.Length == 10 && "6789".IndexOf(d[0]) >= 0 ? d : null;
        }

        public static bool IsName(string name) { return !string.IsNullOrEmpty(name) && NameRx.IsMatch(name); }
        public static bool IsPincode(string pin) { return !string.IsNullOrEmpty(pin) && PinRx.IsMatch(pin); }
        public static bool IsPnr(string pnr) { return !string.IsNullOrEmpty(pnr) && PnrRx.IsMatch(pnr); }

        public static string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || email.IndexOf('@') < 1) return "your e-mail";
            string[] p = email.Split('@');
            string local = p[0].Length <= 2 ? p[0].Substring(0, 1) + "*" : p[0].Substring(0, 2) + new string('*', Math.Min(6, p[0].Length - 2));
            return local + "@" + p[1];
        }

        public static string MaskMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile) || mobile.Length < 4) return "your mobile";
            return new string('*', mobile.Length - 3) + mobile.Substring(mobile.Length - 3);
        }
    }
}
