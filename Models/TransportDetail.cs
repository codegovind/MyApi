namespace TaxAccount.Models
{
    public enum TransportMode
    {
        Road = 1,
        Rail = 2,
        Air = 3,
        Ship = 4
    }

    public class TransportDetail
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int InvoiceId { get; set; }
        public string? TransporterId { get; set; }
        public string? TransporterName { get; set; }
        public string? VehicleNumber { get; set; }
        public int? Distance { get; set; }
        public string? TransportDocNo { get; set; }
        public TransportMode Mode { get; set; } = TransportMode.Road;

        // Navigation
        public Invoice Invoice { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
    }
}