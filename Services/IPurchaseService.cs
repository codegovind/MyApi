using TaxAccount.DTOs;

namespace TaxAccount.Services
{
    public interface IPurchaseService
    {
        // Purchase Bills
        Task<List<PurchaseBillResponseDto>> GetAllBillsAsync();
        Task<PurchaseBillResponseDto> GetBillByIdAsync(int id);
        Task<PurchaseBillResponseDto> CreateBillAsync(
            CreatePurchaseBillDto dto, int userId);
        Task<bool> DeleteBillAsync(int id);

        // Purchase Orders
        Task<List<PurchaseOrderResponseDto>> GetAllOrdersAsync();
        Task<PurchaseOrderResponseDto> GetOrderByIdAsync(int id);
        Task<PurchaseOrderResponseDto> CreateOrderAsync(
            CreatePurchaseOrderDto dto, int userId);
        Task<PurchaseOrderResponseDto> UpdateOrderStatusAsync(
            int id, UpdatePurchaseOrderStatusDto dto);
        Task<PurchaseBillResponseDto> ConvertOrderToBillAsync(
            int orderId, int userId);
    }
}