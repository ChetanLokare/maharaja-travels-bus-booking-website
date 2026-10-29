using System;
using System.Data;
using BusBookingProject.Security;

namespace BusBookingProject.Services
{
    public class UserRecord
    {
        public int UserId;
        public string FirstName, LastName, Email, Mobile, PasswordHash, LegacyPassword;
        public bool EmailVerified, MobileVerified, IsActive;
        public int FailedAttempts;
        public DateTime? LockoutEnd;
        public string DisplayName { get { return (FirstName + " " + LastName).Trim(); } }
        public bool IsLocked { get { return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.Now; } }
    }

    public static class UserRepository
    {
        private const string Cols = "regId, Fname, Lname, EmailId, Contact, PasswordHash, [Password], EmailVerified, MobileVerified, IsActive, FailedAttempts, LockoutEnd";

        private static UserRecord Map(DataRow r)
        {
            return new UserRecord
            {
                UserId = Convert.ToInt32(r["regId"]),
                FirstName = Convert.ToString(r["Fname"]),
                LastName = Convert.ToString(r["Lname"]),
                Email = Convert.ToString(r["EmailId"]),
                Mobile = Convert.ToString(r["Contact"]),
                PasswordHash = r["PasswordHash"] == DBNull.Value ? null : Convert.ToString(r["PasswordHash"]),
                LegacyPassword = r["Password"] == DBNull.Value ? null : Convert.ToString(r["Password"]),
                EmailVerified = Convert.ToBoolean(r["EmailVerified"]),
                MobileVerified = Convert.ToBoolean(r["MobileVerified"]),
                IsActive = Convert.ToBoolean(r["IsActive"]),
                FailedAttempts = Convert.ToInt32(r["FailedAttempts"]),
                LockoutEnd = r["LockoutEnd"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["LockoutEnd"])
            };
        }

        /// <summary>Find by e-mail (contains '@') or by mobile / legacy user name.</summary>
        public static UserRecord FindByIdentifier(string identifier)
        {
            identifier = (identifier ?? "").Trim();
            if (identifier.Length == 0 || identifier.Length > 150) return null;
            DataTable dt;
            if (identifier.Contains("@"))
            {
                dt = Db.Query("SELECT TOP 1 " + Cols + " FROM dbo.Registration WHERE EmailId = @v", Db.P("@v", Validator.NormalizeEmail(identifier)));
            }
            else
            {
                string mobile = Validator.NormalizeMobile(identifier);
                dt = Db.Query("SELECT TOP 1 " + Cols + " FROM dbo.Registration WHERE Contact = @v", Db.P("@v", mobile ?? identifier));
            }
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public static UserRecord FindById(int id)
        {
            var dt = Db.Query("SELECT TOP 1 " + Cols + " FROM dbo.Registration WHERE regId = @v", Db.P("@v", id));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public static bool EmailOrMobileExists(string email, string mobile)
        {
            object o = Db.Scalar("SELECT TOP 1 1 FROM dbo.Registration WHERE EmailId = @e OR (@m IS NOT NULL AND Contact = @m)",
                Db.P("@e", email), Db.P("@m", mobile));
            return o != null;
        }

        public static int Create(string first, string last, string email, string mobile, string address, string city, string pin,
                                 string passwordHash, bool emailVerified)
        {
            object id = Db.Scalar(
                @"INSERT INTO dbo.Registration(Fname, Lname, EmailId, Address, City, Picode, Contact, PasswordHash, EmailVerified, CreatedDate, PasswordChangedAt)
                  OUTPUT INSERTED.regId VALUES(@f,@l,@e,@a,@c,@p,@m,@h,@v,GETDATE(),GETDATE())",
                Db.P("@f", first), Db.P("@l", last), Db.P("@e", email), Db.P("@a", address), Db.P("@c", city), Db.P("@p", pin),
                Db.P("@m", mobile), Db.P("@h", passwordHash), Db.P("@v", emailVerified));
            return Convert.ToInt32(id);
        }

        public static void RecordFailure(int userId)
        {
            int max = Config.GetInt("Security:MaxFailedLogins", 5);
            int minutes = Config.GetInt("Security:LockoutMinutes", 15);
            Db.NonQuery(@"UPDATE dbo.Registration
                          SET FailedAttempts = FailedAttempts + 1,
                              LockoutEnd = CASE WHEN FailedAttempts + 1 >= @max THEN DATEADD(MINUTE, @min, GETDATE()) ELSE LockoutEnd END
                          WHERE regId = @id", Db.P("@max", max), Db.P("@min", minutes), Db.P("@id", userId));
        }

        public static void ResetLockout(int userId)
        {
            Db.NonQuery("UPDATE dbo.Registration SET FailedAttempts = 0, LockoutEnd = NULL WHERE regId = @id", Db.P("@id", userId));
        }

        public static void RecordSuccess(int userId, string ip)
        {
            Db.NonQuery("UPDATE dbo.Registration SET FailedAttempts = 0, LockoutEnd = NULL, LastLoginAt = GETDATE(), LastLoginIp = @ip WHERE regId = @id",
                Db.P("@id", userId), Db.P("@ip", ip));
        }

        /// <summary>Stores a new hash and wipes the legacy plain-text password.</summary>
        public static void SetPassword(int userId, string hash)
        {
            Db.NonQuery("UPDATE dbo.Registration SET PasswordHash = @h, [Password] = NULL, PasswordChangedAt = GETDATE(), FailedAttempts = 0, LockoutEnd = NULL WHERE regId = @id",
                Db.P("@h", hash), Db.P("@id", userId));
        }

        public static void MarkEmailVerified(int userId)
        {
            Db.NonQuery("UPDATE dbo.Registration SET EmailVerified = 1 WHERE regId = @id", Db.P("@id", userId));
        }

        // ---- social sign-in links ----
        public static int? FindByExternal(string provider, string providerUserId)
        {
            object o = Db.Scalar("SELECT RegId FROM dbo.ExternalLogins WHERE Provider = @p AND ProviderUserId = @u",
                Db.P("@p", provider), Db.P("@u", providerUserId));
            return o == null ? (int?)null : Convert.ToInt32(o);
        }

        public static void LinkExternal(string provider, string providerUserId, int userId, string email)
        {
            Db.NonQuery(@"IF NOT EXISTS (SELECT 1 FROM dbo.ExternalLogins WHERE Provider = @p AND ProviderUserId = @u)
                          INSERT INTO dbo.ExternalLogins(Provider, ProviderUserId, RegId, Email) VALUES(@p,@u,@r,@e)",
                Db.P("@p", provider), Db.P("@u", providerUserId), Db.P("@r", userId), Db.P("@e", email));
        }
    }
}
