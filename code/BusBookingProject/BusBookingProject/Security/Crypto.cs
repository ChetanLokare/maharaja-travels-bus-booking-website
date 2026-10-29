using System;
using System.Security.Cryptography;
using System.Text;

namespace BusBookingProject.Security
{
    /// <summary>Cryptographically secure random values and comparison helpers.</summary>
    public static class Crypto
    {
        private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        public static byte[] RandomBytes(int count)
        {
            var b = new byte[count];
            lock (Rng) { Rng.GetBytes(b); }
            return b;
        }

        /// <summary>URL-safe random token (default 256 bits).</summary>
        public static string RandomToken(int bytes = 32)
        {
            return Base64Url(RandomBytes(bytes));
        }

        public static string Base64Url(byte[] data)
        {
            return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public static byte[] Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            return Convert.FromBase64String(s);
        }

        /// <summary>Uniformly distributed string from the given alphabet (rejection sampling - no modulo bias).</summary>
        public static string RandomString(int length, string alphabet)
        {
            int limit = 256 - (256 % alphabet.Length);
            var sb = new StringBuilder(length);
            var buf = new byte[length * 2];
            while (sb.Length < length)
            {
                lock (Rng) { Rng.GetBytes(buf); }
                for (int i = 0; i < buf.Length && sb.Length < length; i++)
                {
                    if (buf[i] < limit) sb.Append(alphabet[buf[i] % alphabet.Length]);
                }
            }
            return sb.ToString();
        }

        public static string RandomDigits(int length)
        {
            return RandomString(length, "0123456789");
        }

        public static string HmacSha256Hex(string key, string data)
        {
            using (var h = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                return ToHex(h.ComputeHash(Encoding.UTF8.GetBytes(data)));
            }
        }

        public static string Sha256Hex(string data)
        {
            using (var h = SHA256.Create())
            {
                return ToHex(h.ComputeHash(Encoding.UTF8.GetBytes(data)));
            }
        }

        public static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>Constant-time comparison (does not leak how many leading characters matched).</summary>
        public static bool FixedTimeEquals(string a, string b)
        {
            if (a == null || b == null) return false;
            byte[] x = Encoding.UTF8.GetBytes(a);
            byte[] y = Encoding.UTF8.GetBytes(b);
            int diff = x.Length ^ y.Length;
            for (int i = 0; i < x.Length && i < y.Length; i++) diff |= x[i] ^ y[i];
            return diff == 0;
        }
    }
}
