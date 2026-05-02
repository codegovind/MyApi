namespace TaxAccount.Models
{
    public enum AdjustmentType
    {
        AuditMatch = 1,
        Damage = 2,
        Devaluation = 3,
        OpeningBalance = 4
    }

    public class StockAdjustment
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; } // negative = loss, positive = found
        public AdjustmentType AdjustmentType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime AdjustmentDate { get; set; } = DateTime.UtcNow;
        public int AdjustedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Product Product { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
        public User AdjustedBy { get; set; } = null!;
    }
}