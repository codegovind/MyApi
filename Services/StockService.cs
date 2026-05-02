using Microsoft.EntityFrameworkCore;
using TaxAccount.Data;
using TaxAccount.DTOs;
using TaxAccount.Exceptions;
using TaxAccount.Models;
using TaxAccount.Services;

namespace TaxAccount.Services
{
    public class StockService : IStockService
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly ILogger<StockService> _logger;

        public StockService(
            AppDbContext context,
            ITenantService tenantService,
            ILogger<StockService> logger)
        {
            _context = context;
            _tenantService = tenantService;
            _logger = logger;
        }

        public async Task<StockAdjustmentResponseDto> AdjustStockAsync(
            CreateStockAdjustmentDto dto, int userId)
        {
            var tenantId = _tenantService.GetTenantId();

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

            if (product == null)
                throw new NotFoundException("Product not found");

            await using var transaction = await _context.Database
                .BeginTransactionAsync();

            try
            {
                // Create adjustment record
                var adjustment = new StockAdjustment
                {
                    TenantId = tenantId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    AdjustmentType = dto.AdjustmentType,
                    Reason = dto.Reason,
                    AdjustmentDate = dto.AdjustmentDate,
                    AdjustedByUserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.StockAdjustments.Add(adjustment);

                // Update product stock
                product.Stock += dto.Quantity;

                // Update market value if devaluation
                if (dto.AdjustmentType == AdjustmentType.Devaluation
                    && dto.NewMarketValue.HasValue)
                {
                    product.MarketValue = dto.NewMarketValue.Value;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Stock adjusted for product {ProductId}: " +
                    "{Quantity} units - {Reason}",
                    dto.ProductId, dto.Quantity, dto.Reason);

                return new StockAdjustmentResponseDto
                {
                    Id = adjustment.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = adjustment.Quantity,
                    AdjustmentType = adjustment.AdjustmentType.ToString(),
                    Reason = adjustment.Reason,
                    AdjustmentDate = adjustment.AdjustmentDate,
                    StockAfterAdjustment = product.Stock,
                    ClosingStockValue = product.ClosingStockValue
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Stock adjustment failed");
                throw new AppException("Stock adjustment failed", 500);
            }
        }

        public async Task<List<StockAdjustmentResponseDto>> GetAdjustmentsAsync(
            int productId)
        {
            return await _context.StockAdjustments
                .Where(sa => sa.ProductId == productId)
                .Include(sa => sa.Product)
                .OrderByDescending(sa => sa.AdjustmentDate)
                .Select(sa => new StockAdjustmentResponseDto
                {
                    Id = sa.Id,
                    ProductId = sa.ProductId,
                    ProductName = sa.Product.Name,
                    Quantity = sa.Quantity,
                    AdjustmentType = sa.AdjustmentType.ToString(),
                    Reason = sa.Reason,
                    AdjustmentDate = sa.AdjustmentDate,
                    StockAfterAdjustment = sa.Product.Stock,
                    ClosingStockValue = sa.Product.ClosingStockValue
                })
                .ToListAsync();
        }

        public async Task<decimal> GetCurrentStockAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new NotFoundException("Product not found");

            return product.Stock;
        }
    }
}