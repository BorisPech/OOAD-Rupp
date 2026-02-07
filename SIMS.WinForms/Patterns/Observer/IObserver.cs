namespace SIMS.WinForms.Patterns.Observer
{
    // Non-generic to avoid conflict with System.IObserver<T>
    public interface IObserver
    {
        void Update(string message);
    }
}
