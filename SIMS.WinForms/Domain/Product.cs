namespace SIMS.WinForms.Domain
{
    public sealed class Product
    {
        public long Id { get; set; }

        public string Code { get; set; }      // e.g. P-001
        public string Name { get; set; }      // e.g. Coca Cola
        public string Category { get; set; }  // e.g. Beverage

        public decimal Cost { get; set; }
        public decimal Price { get; set; }

        public int Stock { get; set; }        // Current quantity in stock
        public int ReorderLevel { get; set; } // Low-stock threshold

        public bool IsActive { get; set; }

        public Product()
        {
            Code = "";
            Name = "";
            Category = "";
            IsActive = true;
        }

        public override string ToString()
        {
            return Code + " - " + Name;
        }
    }
}