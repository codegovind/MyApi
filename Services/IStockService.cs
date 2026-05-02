using TaxAccount.DTOs;

namespace TaxAccount.Services
{
    public interface IStockService
    {
        Task<StockAdjustmentResponseDto> AdjustStockAsync(
            CreateStockAdjustmentDto dto, int userId);
        Task<List<StockAdjustmentResponseDto>> GetAdjustmentsAsync(
            int productId);
        Task<decimal> GetCurrentStockAsync(int productId);
    }
}