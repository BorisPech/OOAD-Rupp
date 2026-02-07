namespace SIMS.WinForms.Domain
{
    public sealed class Customer
    {
        public long Id { get; set; }
        public string Code { get; set; } = "";
        public string FullName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
