using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace SIMS.WinForms.Data
{
    // ✅ Singleton (Design Pattern) - One connection factory across app
    public sealed class SqliteConnectionFactory
    {
        private static readonly Lazy<SqliteConnectionFactory> _instance =
            new Lazy<SqliteConnectionFactory>(() => new SqliteConnectionFactory());

        public static SqliteConnectionFactory Instance => _instance.Value;

        private readonly string _connectionString;

        private SqliteConnectionFactory()
        {
            // DB file at: SIMS.WinForms/bin/Debug/net8.0-windows/sims.db
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sims.db");
            _connectionString = $"Data Source={dbPath};Cache=Shared";
        }

        public SqliteConnection CreateOpen()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }
        public static Microsoft.Data.Sqlite.SqliteConnection CreateOpen() => Instance.CreateOpen();
    }
}
