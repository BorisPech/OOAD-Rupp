using SIMS.WinForms.Common;
using SIMS.WinForms.Patterns.Observer;

namespace SIMS.WinForms.Services
{
    public sealed class NotificationService : IObserver
    {
        public void Update(string message)
        {
            // Show toast notification
            Toast.Info(message);
        }
    }
}
