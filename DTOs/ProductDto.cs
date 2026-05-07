using System.ComponentModel.DataAnnotations;
namespace TaxAccount.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string HsnCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PurchasePrice { get; set; }
        public decimal MarketValue { get; set; }
        public decimal Price { get; set; }
        public decimal Stock { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal GSTPercent { get; set; }
        public bool IsActive { get; set; }
        public decimal ClosingStockValue { get; set; }
    }
}