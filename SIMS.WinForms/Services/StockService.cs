using System.Collections.Generic;
using SIMS.WinForms.Data.Repositories;
using SIMS.WinForms.Domain;
using SIMS.WinForms.Patterns.Observer;

namespace SIMS.WinForms.Services
{
    public sealed class StockService
    {
        private readonly StockRepository _repo = new StockRepository();

        // default: publish = true (for quick notify in main actions)
        public List<Product> GetLowStock()
        {
            return GetLowStock(true);
        }

        // allow UI pages to refresh quietly (no toast spam)
        public List<Product> GetLowStock(bool publish)
        {
            var items = _repo.GetLowStock();
            if (publish) LowStockNotifier.Instance.PublishLowStock(items.Count);
            return items;
        }

        public void AdjustStock(long productId, int delta)
        {
            _repo.AdjustStock(productId, delta);
        }
    }
}
