using System.ComponentModel.DataAnnotations;
using TaxAccount.Models;

namespace TaxAccount.DTOs
{
    public class CreateStockAdjustmentDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public AdjustmentType AdjustmentType { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public DateTime AdjustmentDate { get; set; } = DateTime.UtcNow;

        // Optional - only for Devaluation type
        public decimal? NewMarketValue { get; set; }
    }

    public class StockAdjustmentResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string AdjustmentType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime AdjustmentDate { get; set; }
        public decimal StockAfterAdjustment { get; set; }
        public decimal ClosingStockValue { get; set; }
    }
}