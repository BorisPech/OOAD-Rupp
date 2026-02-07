namespace SIMS.WinForms.Patterns.Strategy
{
    public sealed class FixedAmountDiscountStrategy : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal subTotal, decimal discountValue)
        {
            if (subTotal <= 0m) return 0m;
            if (discountValue <= 0m) return 0m;

            // discountValue = fixed money
            if (discountValue > subTotal) discountValue = subTotal;

            return discountValue;
        }
    }
}
