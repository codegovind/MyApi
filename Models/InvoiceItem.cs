namespace TaxAccount.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string HsnCode { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "Nos";
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }

        // GST breakdown snapshot
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
        public Invoice Invoice { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}