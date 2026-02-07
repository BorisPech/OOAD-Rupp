using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SIMS.WinForms.Domain;

namespace SIMS.WinForms.Data.Repositories
{
    public sealed class SaleRepository
    {
        public long AddSaleWithItems(Sale sale)
        {
            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    long saleId;

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"
INSERT INTO Sales(InvoiceNo,SaleDate,CustomerId,CustomerName,DiscountType,DiscountValue,SubTotal,DiscountAmount,GrandTotal)
VALUES(@InvoiceNo,@SaleDate,@CustomerId,@CustomerName,@DiscountType,@DiscountValue,@SubTotal,@DiscountAmount,@GrandTotal);
SELECT last_insert_rowid();
";
                        cmd.Parameters.AddWithValue("@InvoiceNo", sale.InvoiceNo);
                        cmd.Parameters.AddWithValue("@SaleDate", sale.SaleDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
                        cmd.Parameters.AddWithValue("@CustomerName", sale.CustomerName);
                        cmd.Parameters.AddWithValue("@DiscountType", (int)sale.DiscountType);
                        cmd.Parameters.AddWithValue("@DiscountValue", sale.DiscountValue);
                        cmd.Parameters.AddWithValue("@SubTotal", sale.SubTotal);
                        cmd.Parameters.AddWithValue("@DiscountAmount", sale.DiscountAmount);
                        cmd.Parameters.AddWithValue("@GrandTotal", sale.GrandTotal);

                        saleId = (long)cmd.ExecuteScalar();
                    }

                    foreach (var it in sale.Items)
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = @"
INSERT INTO SaleItems(SaleId,ProductId,ProductCode,ProductName,UnitPrice,Qty)
VALUES(@SaleId,@ProductId,@ProductCode,@ProductName,@UnitPrice,@Qty);
";
                            cmd.Parameters.AddWithValue("@SaleId", saleId);
                            cmd.Parameters.AddWithValue("@ProductId", it.ProductId);
                            cmd.Parameters.AddWithValue("@ProductCode", it.ProductCode);
                            cmd.Parameters.AddWithValue("@ProductName", it.ProductName);
                            cmd.Parameters.AddWithValue("@UnitPrice", it.UnitPrice);
                            cmd.Parameters.AddWithValue("@Qty", it.Qty);
                            cmd.ExecuteNonQuery();
                        }

                        // reduce stock
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = "UPDATE Products SET Stock = Stock - @Qty WHERE Id=@Pid;";
                            cmd.Parameters.AddWithValue("@Qty", it.Qty);
                            cmd.Parameters.AddWithValue("@Pid", it.ProductId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                    return saleId;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public List<Sale> GetRecentSales(int top = 50)
        {
            var list = new List<Sale>();

            using (var conn = SqliteConnectionFactory.Instance.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT Id,InvoiceNo,SaleDate,CustomerId,CustomerName,DiscountType,DiscountValue,SubTotal,DiscountAmount,GrandTotal
FROM Sales
ORDER BY Id DESC
LIMIT @Top;
";
                cmd.Parameters.AddWithValue("@Top", top);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var s = new Sale();
                        s.Id = r.GetInt64(0);
                        s.InvoiceNo = r.GetString(1);
                        s.SaleDate = DateTime.Parse(r.GetString(2));
                        s.CustomerId = r.GetInt64(3);
                        s.CustomerName = r.GetString(4);
                        s.DiscountType = (SIMS.WinForms.Domain.Enums.DiscountType)r.GetInt32(5);
                        s.DiscountValue = Convert.ToDecimal(r.GetDouble(6));
                        s.SubTotal = Convert.ToDecimal(r.GetDouble(7));
                        s.DiscountAmount = Convert.ToDecimal(r.GetDouble(8));
                        s.GrandTotal = Convert.ToDecimal(r.GetDouble(9));
                        list.Add(s);
                    }
                }
            }

            return list;
        }
    }
}
