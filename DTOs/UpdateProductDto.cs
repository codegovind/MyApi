using System.ComponentModel.DataAnnotations;
namespace TaxAccount.DTOs
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;
        public string HsnCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Range(0, 9999999)]
        public decimal PurchasePrice { get; set; } = 0;

        [Range(0, 9999999)]
        public decimal MarketValue { get; set; } = 0;

        [Required]
        [Range(0.01, 9999999)]
        public decimal Price { get; set; }

        public string Unit { get; set; } = "Nos";

        [Range(0, 100)]
        public decimal GSTPercent { get; set; } = 18;

        public bool IsActive { get; set; } = true;
    }
}