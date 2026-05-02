using System.ComponentModel.DataAnnotations;

namespace TaxAccount.DTOs
{
    public class CreateInvoiceItemDto
    {
        [Required]
        public int ProductId { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 99999)]
        public decimal Quantity { get; set; }

        [Range(0, 9999999)]
        public decimal UnitPrice { get; set; } = 0;

        [Range(0, 100)]
        public decimal DiscountPercent { get; set; } = 0;

        [Range(0, 100)]
        public decimal TaxPercent { get; set; } = 0;
    }

    public class InvoiceItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HsnCode { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
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
    }
}