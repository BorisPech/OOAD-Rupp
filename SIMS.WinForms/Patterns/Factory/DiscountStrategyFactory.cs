using SIMS.WinForms.Domain.Enums;
using SIMS.WinForms.Patterns.Strategy;

namespace SIMS.WinForms.Patterns.Factory
{
    // Factory: returns the correct discount strategy
    public static class DiscountStrategyFactory
    {
        public static IDiscountStrategy Create(DiscountType type)
        {
            if (type == DiscountType.Percent) return new PercentDiscountStrategy();
            if (type == DiscountType.FixedAmount) return new FixedAmountDiscountStrategy();
            return new NoDiscountStrategy();
        }
    }
}
