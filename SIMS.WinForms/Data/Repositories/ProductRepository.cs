using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SIMS.WinForms.Domain;

namespace SIMS.WinForms.Data.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        public List<Product> GetAll()
        {
            var list = new List<Product>();

            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Code, Name, Price, StockQty, IsActive FROM Products ORDER BY Id DESC;";
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }

            return list;
        }

        public List<Product> Search(string keyword)
        {
            keyword = (keyword ?? "").Trim();
            var list = new List<Product>();

            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT Id, Code, Name, Price, StockQty, IsActive
FROM Products
WHERE Code LIKE @k OR Name LIKE @k
ORDER BY Id DESC;";
                cmd.Parameters.AddWithValue("@k", "%" + keyword + "%");

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }

            return list;
        }

        public Product GetById(int id)
        {
            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Code, Name, Price, StockQty, IsActive FROM Products WHERE Id=@id;";
                cmd.Parameters.AddWithValue("@id", id);

                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read()) return Map(r);
                }
            }
            return null;
        }

        public int Insert(Product p)
        {
            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO Products(Code, Name, Price, StockQty, IsActive)
VALUES(@Code, @Name, @Price, @StockQty, @IsActive);
SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("@Code", p.Code ?? "");
                cmd.Parameters.AddWithValue("@Name", p.Name ?? "");
                cmd.Parameters.AddWithValue("@Price", p.Price);
                cmd.Parameters.AddWithValue("@StockQty", p.StockQty);
                cmd.Parameters.AddWithValue("@IsActive", p.IsActive ? 1 : 0);

                // ✅ SQLite returns long
                var id = (long)cmd.ExecuteScalar();
                return checked((int)id);
            }
        }

        public void Update(Product p)
        {
            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE Products SET
    Code=@Code,
    Name=@Name,
    Price=@Price,
    StockQty=@StockQty,
    IsActive=@IsActive
WHERE Id=@Id;";

                cmd.Parameters.AddWithValue("@Id", p.Id);
                cmd.Parameters.AddWithValue("@Code", p.Code ?? "");
                cmd.Parameters.AddWithValue("@Name", p.Name ?? "");
                cmd.Parameters.AddWithValue("@Price", p.Price);
                cmd.Parameters.AddWithValue("@StockQty", p.StockQty);
                cmd.Parameters.AddWithValue("@IsActive", p.IsActive ? 1 : 0);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Products WHERE Id=@id;";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // Optional alias (if you used repo.Add in old code)
        public int Add(Product p) => Insert(p);

        private Product Map(SqliteDataReader r)
        {
            return new Product
            {
                Id = r.GetInt32(0),
                Code = r.GetString(1),
                Name = r.GetString(2),
                Price = r.GetDecimal(3),
                StockQty = r.GetInt32(4),
                IsActive = r.GetInt32(5) == 1
            };
        }
    }
}
