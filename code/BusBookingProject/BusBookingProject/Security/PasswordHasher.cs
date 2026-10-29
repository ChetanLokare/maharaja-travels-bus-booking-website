using System;
using System.Linq;
using System.Security.Cryptography;

namespace BusBookingProject.Security
{
    /// <summary>
    /// Salted PBKDF2-HMAC-SHA256. Stored format:  PBKDF2-SHA256$iterations$salt(base64)$hash(base64)
    /// The iteration count is stored with every hash, so it can be raised later without locking anyone out.
    /// </summary>
    public static class PasswordHasher
    {
        private const string Prefix = "PBKDF2-SHA256";
        private const int SaltSize = 16;
        private const int KeySize = 32;

        private static int CurrentIterations
        {
            get { return Math.Max(100000, Config.GetInt("Security:Pbkdf2Iterations", 600000)); }
        }

        public static string Hash(string password)
        {
            byte[] salt = Crypto.RandomBytes(SaltSize);
            int iterations = CurrentIterations;
            byte[] key = Derive(password, salt, iterations, KeySize);
            return Prefix + "$" + iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(key);
        }

        public static bool Verify(string password, string stored, out bool needsRehash)
        {
            needsRehash = false;
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored)) return false;

            string[] parts = stored.Split('$');
            int iterations;
            if (parts.Length != 4 || parts[0] != Prefix || !int.TryParse(parts[1], out iterations) || iterations < 1000) return false;

            try
            {
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expected = Convert.FromBase64String(parts[3]);
                byte[] actual = Derive(password, salt, iterations, expected.Length);

                int diff = 0;
                for (int i = 0; i < expected.Length; i++) diff |= expected[i] ^ actual[i];
                bool ok = diff == 0;
                needsRehash = ok && iterations < CurrentIterations;
                return ok;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>Spend roughly the same time as a real check, so "unknown account" is not distinguishable by timing.</summary>
        public static void BurnTime()
        {
            Derive("burn-time-dummy-password", new byte[SaltSize], CurrentIterations, KeySize);
        }

        private static byte[] Derive(string password, byte[] salt, int iterations, int length)
        {
            using (var kdf = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return kdf.GetBytes(length);
            }
        }

        // ---- password policy ----
        private static readonly string[] Common =
        {
            "password","password1","password123","12345678","123456789","1234567890","qwerty123","qwertyuiop","iloveyou",
            "admin123","welcome1","welcome123","letmein123","abc12345","india123","maharaja","maharaja123","passw0rd","p@ssw0rd","user1234"
        };

        /// <summary>Returns null when the password is acceptable, otherwise a message for the user.</summary>
        public static string CheckPolicy(string password, string email)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 10) return "Password must be at least 10 characters long.";
            if (password.Length > 128) return "Password must be 128 characters or fewer.";
            if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
                return "Password needs an uppercase letter, a lowercase letter and a number.";
            string lower = password.ToLowerInvariant();
            if (Common.Any(c => lower.Contains(c))) return "That password is too common. Please choose something less guessable.";
            if (!string.IsNullOrEmpty(email))
            {
                string local = email.Split('@')[0].ToLowerInvariant();
                if (local.Length >= 4 && lower.Contains(local)) return "Password must not contain your e-mail name.";
            }
            return null;
        }
    }
}
