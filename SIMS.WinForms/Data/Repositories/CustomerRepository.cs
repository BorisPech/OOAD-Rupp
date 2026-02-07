using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SIMS.WinForms.Domain;

namespace SIMS.WinForms.Data.Repositories
{
    public class CustomerRepository : IRepository<Customer>
    {
        public List<Customer> GetAll()
        {
            var list = new List<Customer>();

            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Code, FullName, Phone, Address, IsActive FROM Customers ORDER BY Id DESC;";

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(Map(r));
                    }
                }
            }

            return list;
        }

        public List<Customer> Search(string keyword)
        {
            keyword = (keyword ?? "").Trim();
            var list = new List<Customer>();

            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT Id, Code, FullName, Phone, Address, IsActive
FROM Customers
WHERE Code LIKE @k OR FullName LIKE @k OR Phone LIKE @k
ORDER BY Id DESC;";
                cmd.Parameters.AddWithValue("@k", "%" + keyword + "%");

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(Map(r));
                    }
                }
            }

            return list;
        }

        public Customer GetById(int id)
        {
            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Code, FullName, Phone, Address, IsActive FROM Customers WHERE Id=@id;";
                cmd.Parameters.AddWithValue("@id", id);

                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read()) return Map(r);
                }
            }

            return null;
        }

        public int Insert(Customer c)
        {
            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO Customers(Code, FullName, Phone, Address, IsActive)
VALUES(@Code, @FullName, @Phone, @Address, @IsActive);
SELECT last_insert_rowid();";
                cmd.Parameters.AddWithValue("@Code", c.Code ?? "");
                cmd.Parameters.AddWithValue("@FullName", c.FullName ?? "");
                cmd.Parameters.AddWithValue("@Phone", c.Phone ?? "");
                cmd.Parameters.AddWithValue("@Address", c.Address ?? "");
                cmd.Parameters.AddWithValue("@IsActive", c.IsActive ? 1 : 0);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void Update(Customer c)
        {
            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE Customers SET
    Code=@Code,
    FullName=@FullName,
    Phone=@Phone,
    Address=@Address,
    IsActive=@IsActive
WHERE Id=@Id;";
                cmd.Parameters.AddWithValue("@Id", c.Id);
                cmd.Parameters.AddWithValue("@Code", c.Code ?? "");
                cmd.Parameters.AddWithValue("@FullName", c.FullName ?? "");
                cmd.Parameters.AddWithValue("@Phone", c.Phone ?? "");
                cmd.Parameters.AddWithValue("@Address", c.Address ?? "");
                cmd.Parameters.AddWithValue("@IsActive", c.IsActive ? 1 : 0);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Customers WHERE Id=@id;";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        private Customer Map(SqliteDataReader r)
        {
            return new Customer
            {
                Id = r.GetInt32(0),
                Code = r.GetString(1),
                FullName = r.GetString(2),
                Phone = r.GetString(3),
                Address = r.GetString(4),
                IsActive = r.GetInt32(5) == 1
            };
        }
    }
}
