namespace SIMS.WinForms.Patterns.Strategy
{
    // Strategy: discount calculation behavior
    public interface IDiscountStrategy
    {
        decimal CalculateDiscount(decimal subTotal, decimal discountValue);
    }
}
