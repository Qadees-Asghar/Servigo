using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace SERVIGO.Helpers
{
    public static class DatabaseHelper
    {
        private static string _connectionString = string.Empty;

        public static void Initialize()
        {
            string configPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            if (!File.Exists(configPath))
                throw new FileNotFoundException("appsettings.json not found.", configPath);

            string json  = File.ReadAllText(configPath);
            var    doc   = JsonDocument.Parse(json);
            _connectionString = doc.RootElement
                .GetProperty("ConnectionStrings")
                .GetProperty("DefaultConnection")
                .GetString()!;
        }

        public static SqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                Initialize();
            return new SqlConnection(_connectionString);
        }

        // ── Scalar helpers ───────────────────────────────────────────────────

        public static object? ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteScalar();
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }

        public static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd    = new SqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        // ── Stored procedure helpers ─────────────────────────────────────────

        public static DataTable ExecuteStoredProcedure(string procName,
            params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddRange(parameters);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public static object? ExecuteStoredProcedureScalar(string procName,
            params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteScalar();
        }

        public static int ExecuteStoredProcedureNonQuery(string procName,
            params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }

        // ── Convenience parameter factory ────────────────────────────────────

        public static SqlParameter Param(string name, object? value)
            => new(name, value ?? DBNull.Value);

        // ── Connectivity test ────────────────────────────────────────────────

        public static bool TestConnection(out string error)
        {
            error = string.Empty;
            try
            {
                using var conn = GetConnection();
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
