namespace TaxAccount.Models
{
    public enum PurchaseOrderStatus
    {
        Draft = 1,
        Sent = 2,
        Received = 3,
        Cancelled = 4
    }

    public class PurchaseOrder
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedDate { get; set; }
        public PurchaseOrderStatus Status { get; set; } 
            = PurchaseOrderStatus.Draft;
        public int? ContactId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Contact? Contact { get; set; }
        public User CreatedBy { get; set; } = null!;
        public ICollection<PurchaseOrderItem> Items { get; set; } 
            = new List<PurchaseOrderItem>();
    }

    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string HsnCode { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "Nos";
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal CgstPercent { get; set; }
        public decimal CgstAmount { get; set; }
        public decimal SgstPercent { get; set; }
        public decimal SgstAmount { get; set; }
        public decimal IgstPercent { get; set; }
        public decimal IgstAmount { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}