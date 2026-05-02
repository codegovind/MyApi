namespace TaxAccount.Models
{
    public enum GstType
    {
        Registered = 1,
        Unregistered = 2,
        Composition = 3,
        Consumer = 4
    }

    public enum ContactType
    {
        Customer = 1,
        Vendor = 2,
        Both = 3
    }

    public class Contact
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Gstin { get; set; }
        public GstType GstType { get; set; } = GstType.Unregistered;
        public ContactType ContactType { get; set; } = ContactType.Customer;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public decimal OpeningBalance { get; set; } = 0;
        public bool IsDefault { get; set; } = false; // Default Cash Customer
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Tenant Tenant { get; set; } = null!;
    }
}