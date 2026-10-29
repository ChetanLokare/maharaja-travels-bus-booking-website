using System;
using System.Runtime.Caching;
using System.Web;

namespace BusBookingProject.Security
{
    /// <summary>Small in-memory fixed-window rate limiter (per web-server process).</summary>
    public static class Throttle
    {
        private static readonly object Sync = new object();

        private class Counter { public int Count; }

        /// <summary>Returns false when "key" has already been used "max" times inside the window.</summary>
        public static bool Allow(string key, int max, TimeSpan window)
        {
            lock (Sync)
            {
                var cache = MemoryCache.Default;
                var c = cache.Get("thr:" + key) as Counter;
                if (c == null)
                {
                    cache.Set("thr:" + key, new Counter { Count = 1 }, DateTimeOffset.UtcNow.Add(window));
                    return true;
                }
                if (c.Count >= max) return false;
                c.Count++;
                return true;
            }
        }

        public static string ClientIp(HttpRequest request)
        {
            if (Config.GetBool("Security:TrustProxyHeaders", false))
            {
                string xff = request.Headers["X-Forwarded-For"];
                if (!string.IsNullOrEmpty(xff))
                {
                    string first = xff.Split(',')[0].Trim();
                    if (first.Length > 0 && first.Length <= 45) return first;
                }
            }
            return request.UserHostAddress ?? "unknown";
        }
    }
}
