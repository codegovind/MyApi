using System.ComponentModel.DataAnnotations;

namespace TaxAccount.DTOs
{
    public class CreateInvoiceItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 99999)]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0.01, 9999999)]
        public decimal UnitPrice { get; set; }

        [Range(0, 100)]
        public decimal TaxPercent { get; set; } = 18; // Default GST 18%
    }

    public class InvoiceItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}