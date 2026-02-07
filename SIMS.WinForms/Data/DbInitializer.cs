using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace SIMS.WinForms.Data
{
    public static class DbInitializer
    {
        public static void Initialize()
        {
            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            {
                EnsureCustomersTable(conn);
                SeedCustomers(conn);
            }
        }

        private static void EnsureCustomersTable(SqliteConnection conn)
        {
            // 1) Create table if not exists (full schema)
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Customers(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Code TEXT NOT NULL DEFAULT '',
    FullName TEXT NOT NULL DEFAULT '',
    Phone TEXT NOT NULL DEFAULT '',
    Address TEXT NOT NULL DEFAULT '',
    IsActive INTEGER NOT NULL DEFAULT 1
);";
                cmd.ExecuteNonQuery();
            }

            // 2) Auto-migration: if old DB missing column, add it
            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "PRAGMA table_info(Customers);";
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        existing.Add(r.GetString(1)); // column name
                    }
                }
            }

            // Add missing columns safely
            AddColumnIfMissing(conn, existing, "Code", "TEXT NOT NULL DEFAULT ''");
            AddColumnIfMissing(conn, existing, "FullName", "TEXT NOT NULL DEFAULT ''");
            AddColumnIfMissing(conn, existing, "Phone", "TEXT NOT NULL DEFAULT ''");
            AddColumnIfMissing(conn, existing, "Address", "TEXT NOT NULL DEFAULT ''");
            AddColumnIfMissing(conn, existing, "IsActive", "INTEGER NOT NULL DEFAULT 1");
        }

        private static void AddColumnIfMissing(SqliteConnection conn, HashSet<string> existing, string column, string typeSql)
        {
            if (existing.Contains(column)) return;

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"ALTER TABLE Customers ADD COLUMN {column} {typeSql};";
                cmd.ExecuteNonQuery();
            }
        }

        private static void SeedCustomers(SqliteConnection conn)
        {
            // Seed only if empty
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(1) FROM Customers;";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0) return;
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO Customers(Code, FullName, Phone, Address, IsActive) VALUES
('C001','Sok Dara','012345678','Phnom Penh',1),
('C002','Chan Vuthy','098765432','Kandal',1),
('C003','Pech Bora','011223344','Siem Reap',1);";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
