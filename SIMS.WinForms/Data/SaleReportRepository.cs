using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace SIMS.WinForms.Data
{
    // Simple DTO for report (not in Domain, keep reporting in Data layer)
    public sealed class SaleReportRow
    {
        public string InvoiceNo { get; set; }
        public string SaleDate { get; set; }
        public string CustomerName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }

        public SaleReportRow()
        {
            InvoiceNo = "";
            SaleDate = "";
            CustomerName = "";
        }
    }

    public sealed class SaleReportRepository
    {
        public List<SaleReportRow> GetSales(DateTime from, DateTime to)
        {
            var list = new List<SaleReportRow>();

            using (var conn = SqliteConnectionFactory.CreateOpen())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT InvoiceNo, SaleDate, CustomerName, SubTotal, DiscountAmount, GrandTotal
FROM Sales
WHERE SaleDate >= @From AND SaleDate <= @To
ORDER BY SaleDate DESC;
";
                cmd.Parameters.AddWithValue("@From", from.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@To", to.ToString("yyyy-MM-dd HH:mm:ss"));

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var row = new SaleReportRow
                        {
                            InvoiceNo = r.GetString(0),
                            SaleDate = r.GetString(1),
                            CustomerName = r.GetString(2),
                            SubTotal = Convert.ToDecimal(r.GetDouble(3)),
                            DiscountAmount = Convert.ToDecimal(r.GetDouble(4)),
                            GrandTotal = Convert.ToDecimal(r.GetDouble(5))
                        };
                        list.Add(row);
                    }
                }
            }

            return list;
        }
    }
}