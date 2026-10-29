using System;
using System.Configuration;
using System.Globalization;

namespace BusBookingProject.Security
{
    /// <summary>Typed access to appSettings.</summary>
    public static class Config
    {
        public static string Get(string key, string fallback = "")
        {
            string v = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(v) ? fallback : v.Trim();
        }

        public static bool GetBool(string key, bool fallback)
        {
            bool b;
            return bool.TryParse(Get(key, ""), out b) ? b : fallback;
        }

        public static int GetInt(string key, int fallback)
        {
            int i;
            return int.TryParse(Get(key, ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out i) ? i : fallback;
        }

        public static string BaseUrl
        {
            get { return Get("App:BaseUrl", "https://localhost").TrimEnd('/'); }
        }
    }
}
