namespace TaxAccount.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string HsnCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Pricing
        public decimal PurchasePrice { get; set; } = 0; // Cost price
        public decimal MarketValue { get; set; } = 0;   // NRV
        public decimal Price { get; set; }               // Selling price

        // Stock
        public decimal Stock { get; set; } = 0;
        public string Unit { get; set; } = "Nos";

        // GST
        public decimal GSTPercent { get; set; } = 18;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Closing stock value = Qty * Min(PurchasePrice, MarketValue)
        public decimal ClosingStockValue =>
            Stock * Math.Min(PurchasePrice, MarketValue);

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public ICollection<StockAdjustment> StockAdjustments { get; set; }
            = new List<StockAdjustment>();
    }
}