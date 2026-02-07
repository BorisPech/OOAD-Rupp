using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SIMS.WinForms.Domain;

namespace SIMS.WinForms.Data.Repositories
{
    public sealed class StockRepository
    {
        public List<Product> GetLowStock()
        {
            var list = new List<Product>();

            using (var conn = SqliteConnectionFactory.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT Id,Code,Name,Category,Cost,Price,Stock,ReorderLevel,IsActive
FROM Products
WHERE IsActive=1 AND Stock <= ReorderLevel
ORDER BY Stock ASC;
";
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var p = new Product();
                        p.Id = r.GetInt64(0);
                        p.Code = r.GetString(1);
                        p.Name = r.GetString(2);
                        p.Category = r.GetString(3);
                        p.Cost = Convert.ToDecimal(r.GetDouble(4));
                        p.Price = Convert.ToDecimal(r.GetDouble(5));
                        p.Stock = r.GetInt32(6);
                        p.ReorderLevel = r.GetInt32(7);
                        p.IsActive = r.GetInt32(8) == 1;
                        list.Add(p);
                    }
                }
            }

            return list;
        }

        public void AdjustStock(long productId, int delta)
        {
            using (var conn = SqliteConnectionFactory.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE Products SET Stock = Stock + @Delta WHERE Id=@Id;";
                cmd.Parameters.AddWithValue("@Delta", delta);
                cmd.Parameters.AddWithValue("@Id", productId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
