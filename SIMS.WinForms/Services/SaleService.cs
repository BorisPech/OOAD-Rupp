using System;
using System.Collections.Generic;
using SIMS.WinForms.Data.Repositories;
using SIMS.WinForms.Domain;
using SIMS.WinForms.Domain.Enums;
using SIMS.WinForms.Patterns.Factory;

namespace SIMS.WinForms.Services
{
    public sealed class SaleService
    {
        private readonly SaleRepository _saleRepo = new SaleRepository();

        public string GenerateInvoiceNo()
        {
            // Simple & stable invoice format
            // INV-YYYY-xxxxx (based on ticks fragment)
            var yyyy = DateTime.Now.Year;
            var seq = (DateTime.Now.Ticks % 100000).ToString().PadLeft(5, '0');
            return "INV-" + yyyy + "-" + seq;
        }

        public void RecalculateTotals(Sale sale)
        {
            if (sale == null) return;

            decimal sub = 0m;
            for (int i = 0; i < sale.Items.Count; i++)
                sub += sale.Items[i].LineTotal;

            sale.SubTotal = sub;

            var strategy = DiscountStrategyFactory.Create(sale.DiscountType);
            sale.DiscountAmount = strategy.CalculateDiscount(sub, sale.DiscountValue);

            sale.GrandTotal = sub - sale.DiscountAmount;
            if (sale.GrandTotal < 0m) sale.GrandTotal = 0m;
        }

        public long SaveSale(Sale sale)
        {
            ValidateSale(sale);
            RecalculateTotals(sale);

            if (string.IsNullOrWhiteSpace(sale.InvoiceNo))
                sale.InvoiceNo = GenerateInvoiceNo();

            return _saleRepo.AddSaleWithItems(sale);
        }

        public List<Sale> GetRecentSales(int top = 50) => _saleRepo.GetRecentSales(top);

        private static void ValidateSale(Sale sale)
        {
            if (sale == null) throw new Exception("Sale is null.");
            if (sale.CustomerId <= 0) throw new Exception("Please select a customer.");
            if (sale.Items == null || sale.Items.Count == 0) throw new Exception("Sale items are empty.");

            // Validate qty
            for (int i = 0; i < sale.Items.Count; i++)
            {
                if (sale.Items[i].ProductId <= 0) throw new Exception("Invalid product in sale item.");
                if (sale.Items[i].Qty <= 0) throw new Exception("Qty must be >= 1.");
                if (sale.Items[i].UnitPrice < 0) throw new Exception("Unit price must be >= 0.");
            }

            // DiscountType sanity
            if (sale.DiscountType != DiscountType.None &&
                sale.DiscountType != DiscountType.Percent &&
                sale.DiscountType != DiscountType.FixedAmount)
            {
                sale.DiscountType = DiscountType.None;
                sale.DiscountValue = 0m;
            }
        }
    }
}
