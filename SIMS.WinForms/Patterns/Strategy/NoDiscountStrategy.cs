namespace SIMS.WinForms.Patterns.Strategy
{
    public sealed class NoDiscountStrategy : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal subTotal, decimal discountValue)
        {
            return 0m;
        }
    }
}
