namespace SIMS.WinForms.Patterns.Strategy
{
    public sealed class PercentDiscountStrategy : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal subTotal, decimal discountValue)
        {
            if (subTotal <= 0m) return 0m;
            if (discountValue <= 0m) return 0m;

            // discountValue = percent (0..100)
            if (discountValue > 100m) discountValue = 100m;

            var discount = subTotal * (discountValue / 100m);
            if (discount < 0m) discount = 0m;
            if (discount > subTotal) discount = subTotal;

            return discount;
        }
    }
}
