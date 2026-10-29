using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BusBookingProject.Services
{
    /// <summary>Thin ADO.NET helper. Every statement is parameterised - never concatenate user input into SQL.</summary>
    public static class Db
    {
        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["OnlineBusBookingConnectionString"].ConnectionString; }
        }

        public static SqlConnection Open()
        {
            var c = new SqlConnection(ConnectionString);
            c.Open();
            return c;
        }

        public static SqlParameter P(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        public static SqlParameter P(string name, SqlDbType type, int size, object value)
        {
            var p = new SqlParameter(name, type, size);
            p.Value = value ?? DBNull.Value;
            return p;
        }

        public static DataTable Query(string sql, params SqlParameter[] ps)
        {
            using (var c = Open())
            using (var cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                return dt;
            }
        }

        public static int NonQuery(string sql, params SqlParameter[] ps)
        {
            using (var c = Open())
            using (var cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                return cmd.ExecuteNonQuery();
            }
        }

        public static object Scalar(string sql, params SqlParameter[] ps)
        {
            using (var c = Open())
            using (var cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                object o = cmd.ExecuteScalar();
                return o == DBNull.Value ? null : o;
            }
        }

        public static DataSet Proc(string name, params SqlParameter[] ps)
        {
            using (var c = Open())
            using (var cmd = new SqlCommand(name, c) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddRange(ps);
                var ds = new DataSet();
                new SqlDataAdapter(cmd).Fill(ds);
                return ds;
            }
        }

        public static void Audit(int? userId, string ev, string detail, string ip, string userAgent)
        {
            try
            {
                NonQuery("INSERT INTO dbo.AuditLog(UserId, Event, Detail, Ip, UserAgent) VALUES(@u,@e,@d,@ip,@ua)",
                    P("@u", userId), P("@e", ev),
                    P("@d", detail != null && detail.Length > 400 ? detail.Substring(0, 400) : detail),
                    P("@ip", ip), P("@ua", userAgent != null && userAgent.Length > 250 ? userAgent.Substring(0, 250) : userAgent));
            }
            catch { /* auditing must never break a request */ }
        }
    }
}
