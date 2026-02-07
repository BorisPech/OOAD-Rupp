using System;
using System.Collections.Generic;

namespace SIMS.WinForms.Patterns.Observer
{
    // Subject + Singleton: central notifier for low-stock events
    public sealed class LowStockNotifier : ISubject
    {
        private static readonly Lazy<LowStockNotifier> _lazy =
            new Lazy<LowStockNotifier>(() => new LowStockNotifier());

        public static LowStockNotifier Instance => _lazy.Value;

        private readonly List<IObserver> _observers = new List<IObserver>();

        private LowStockNotifier() { }

        public void Attach(IObserver observer)
        {
            if (observer == null) return;
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            if (observer == null) return;
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }

        public void Notify(string message)
        {
            for (int i = 0; i < _observers.Count; i++)
            {
                try { _observers[i].Update(message); }
                catch { /* ignore observer failure */ }
            }
        }

        // Helper method used by StockService later
        public void PublishLowStock(int count)
        {
            if (count <= 0) return;
            Notify("Low stock alert: " + count + " item(s) need restock.");
        }
    }
}
