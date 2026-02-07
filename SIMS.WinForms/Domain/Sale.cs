using System;
using System.Collections.Generic;
using SIMS.WinForms.Domain.Enums;

namespace SIMS.WinForms.Domain
{
    public sealed class Sale
    {
        public long Id { get; set; }

        public string InvoiceNo { get; set; }   // e.g. INV-2026-0001
        public DateTime SaleDate { get; set; }

        public long CustomerId { get; set; }
        public string CustomerName { get; set; }

        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }  // Percent or FixedAmount

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }

        public List<SaleItem> Items { get; set; }

        public Sale()
        {
            InvoiceNo = "";
            CustomerName = "";
            SaleDate = DateTime.Now;
            DiscountType = DiscountType.None;
            Items = new List<SaleItem>();
        }
    }
}