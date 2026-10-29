using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using BusBookingProject.Security;

namespace BusBookingProject.Services
{
    public class OAuthProfile
    {
        public string Provider, Subject, Email, FirstName, LastName;
        public bool EmailVerified;
    }

    /// <summary>
    /// OpenID Connect authorization-code flow with PKCE, state and nonce for Google and Microsoft.
    /// The id_token is received directly from the provider's token endpoint over TLS together with our client
    /// secret, so (per OIDC Core 3.1.3.7) its claims can be validated without separately checking the JWT signature.
    /// </summary>
    public static class OAuthService
    {
        public const string Google = "google";
        public const string Microsoft = "microsoft";

        public static bool IsConfigured(string provider)
        {
            return ClientId(provider).Length > 0 && ClientSecret(provider).Length > 0;
        }

        public static string Normalize(string provider)
        {
            provider = (provider ?? "").ToLowerInvariant();
            return provider == Google || provider == Microsoft ? provider : null;
        }

        private static string ClientId(string p) { return p == Google ? Config.Get("Google:ClientId") : Config.Get("Microsoft:ClientId"); }
        private static string ClientSecret(string p) { return p == Google ? Config.Get("Google:ClientSecret") : Config.Get("Microsoft:ClientSecret"); }
        private static string Tenant { get { return Config.Get("Microsoft:Tenant", "common"); } }

        public static string RedirectUri { get { return Config.BaseUrl + "/OAuthCallback.ashx"; } }

        private static string AuthUrl(string p)
        {
            return p == Google ? "https://accounts.google.com/o/oauth2/v2/auth"
                               : "https://login.microsoftonline.com/" + Tenant + "/oauth2/v2.0/authorize";
        }

        private static string TokenUrl(string p)
        {
            return p == Google ? "https://oauth2.googleapis.com/token"
                               : "https://login.microsoftonline.com/" + Tenant + "/oauth2/v2.0/token";
        }

        public static string CodeChallenge(string verifier)
        {
            using (var sha = SHA256.Create())
                return Crypto.Base64Url(sha.ComputeHash(Encoding.ASCII.GetBytes(verifier)));
        }

        public static string BuildAuthorizeUrl(string provider, string state, string nonce, string codeChallenge)
        {
            var q = new StringBuilder();
            q.Append("client_id=").Append(Uri.EscapeDataString(ClientId(provider)));
            q.Append("&redirect_uri=").Append(Uri.EscapeDataString(RedirectUri));
            q.Append("&response_type=code&scope=").Append(Uri.EscapeDataString("openid email profile"));
            q.Append("&state=").Append(Uri.EscapeDataString(state));
            q.Append("&nonce=").Append(Uri.EscapeDataString(nonce));
            q.Append("&code_challenge=").Append(Uri.EscapeDataString(codeChallenge)).Append("&code_challenge_method=S256");
            q.Append(provider == Google ? "&prompt=select_account" : "&prompt=select_account&response_mode=query");
            return AuthUrl(provider) + "?" + q;
        }

        public static OAuthProfile Exchange(string provider, string code, string codeVerifier, string expectedNonce, out string error)
        {
            error = null;
            try
            {
                string form = "grant_type=authorization_code"
                    + "&code=" + Uri.EscapeDataString(code)
                    + "&redirect_uri=" + Uri.EscapeDataString(RedirectUri)
                    + "&client_id=" + Uri.EscapeDataString(ClientId(provider))
                    + "&client_secret=" + Uri.EscapeDataString(ClientSecret(provider))
                    + "&code_verifier=" + Uri.EscapeDataString(codeVerifier);

                var req = (HttpWebRequest)WebRequest.Create(TokenUrl(provider));
                req.Method = "POST";
                req.Timeout = 20000;
                req.ContentType = "application/x-www-form-urlencoded";
                byte[] data = Encoding.UTF8.GetBytes(form);
                using (Stream s = req.GetRequestStream()) s.Write(data, 0, data.Length);

                string body;
                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var rd = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    body = rd.ReadToEnd();

                var js = new JavaScriptSerializer();
                var token = js.Deserialize<Dictionary<string, object>>(body);
                if (!token.ContainsKey("id_token")) { error = "The provider did not return an identity token."; return null; }

                string[] parts = Convert.ToString(token["id_token"]).Split('.');
                if (parts.Length != 3) { error = "Invalid identity token."; return null; }
                var c = js.Deserialize<Dictionary<string, object>>(Encoding.UTF8.GetString(Crypto.Base64UrlDecode(parts[1])));

                // ---- claim validation ----
                string iss = Str(c, "iss");
                bool issOk = provider == Google
                    ? (iss == "https://accounts.google.com" || iss == "accounts.google.com")
                    : (iss.StartsWith("https://login.microsoftonline.com/") && iss.EndsWith("/v2.0"));
                if (!issOk) { error = "Untrusted token issuer."; return null; }
                if (Str(c, "aud") != ClientId(provider)) { error = "Token was issued for a different application."; return null; }
                long exp = c.ContainsKey("exp") ? Convert.ToInt64(c["exp"]) : 0;
                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > exp) { error = "The sign-in has expired. Please try again."; return null; }
                if (!Crypto.FixedTimeEquals(Str(c, "nonce"), expectedNonce)) { error = "Sign-in could not be verified."; return null; }

                string sub = Str(c, "sub");
                if (sub.Length == 0 || sub.Length > 128) { error = "Invalid account identifier."; return null; }

                string email = Validator.NormalizeEmail(Str(c, "email"));
                if (email.Length == 0 && provider == Microsoft) email = Validator.NormalizeEmail(Str(c, "preferred_username"));
                if (!Validator.IsEmail(email)) { error = "Your " + (provider == Google ? "Google" : "Microsoft") + " account did not share an e-mail address."; return null; }

                // Google asserts verification. Microsoft's e-mail claim is not guaranteed verified -> always confirm by OTP.
                bool verified = provider == Google && string.Equals(Str(c, "email_verified"), "true", StringComparison.OrdinalIgnoreCase);

                string first = Str(c, "given_name"), last = Str(c, "family_name");
                if (first.Length == 0)
                {
                    string[] n = Str(c, "name").Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    first = n.Length > 0 ? n[0] : email.Split('@')[0];
                    last = n.Length > 1 ? n[1] : "";
                }
                return new OAuthProfile
                {
                    Provider = provider, Subject = sub, Email = email, EmailVerified = verified,
                    FirstName = Validator.Clean(first, 50), LastName = Validator.Clean(last, 50)
                };
            }
            catch (Exception)
            {
                error = "We could not complete the sign-in. Please try again.";
                return null;
            }
        }

        private static string Str(Dictionary<string, object> d, string k)
        {
            return d.ContainsKey(k) && d[k] != null ? Convert.ToString(d[k]) : string.Empty;
        }
    }
}
