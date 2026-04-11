using System.ComponentModel.DataAnnotations;
using TaxAccount.Models;

namespace TaxAccount.DTOs
{
    public class CreateInvoiceDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public string Notes { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Invoice must have at least one item")]
        public List<CreateInvoiceItemDto> Items { get; set; } = new();
    }

    public class InvoiceResponseDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<InvoiceItemResponseDto> Items { get; set; } = new();
    }

    public class UpdateInvoiceStatusDto
    {
        [Required]
        public InvoiceStatus Status { get; set; }
    }
}