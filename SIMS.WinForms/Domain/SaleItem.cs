namespace SIMS.WinForms.Domain
{
    public sealed class SaleItem
    {
        public long Id { get; set; }
        public long SaleId { get; set; }

        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }

        public decimal LineTotal
        {
            get { return UnitPrice * Qty; }
        }

        public SaleItem()
        {
            ProductCode = "";
            ProductName = "";
        }
    }
}