using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TaxAccount.Data;
using TaxAccount.DTOs;
using TaxAccount.Exceptions;
using TaxAccount.Helpers;
using TaxAccount.Models;

namespace TaxAccount.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ITenantService _tenantService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            AppDbContext context,
            IMemoryCache cache,
            ITenantService tenantService,
            ILogger<ProductService> logger)
        {
            _context = context;
            _cache = cache;
            _tenantService = tenantService;
            _logger = logger;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var cacheKey = $"{CacheKeys.AllProducts}_{_tenantService.GetTenantId()}";

            if (_cache.TryGetValue(cacheKey, out List<ProductDto>? cached)
                && cached != null)
            {
                _logger.LogInformation("Products retrieved from cache");
                return cached;
            }

            _logger.LogInformation(
                "Cache miss - fetching products from database");

            var products = await _context.Products
                .Where(p => p.IsActive)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    SKU = p.SKU,
                    HsnCode = p.HsnCode,
                    Description = p.Description,
                    PurchasePrice = p.PurchasePrice,
                    MarketValue = p.MarketValue,
                    Price = p.Price,
                    Stock = p.Stock,
                    Unit = p.Unit,
                    GSTPercent = p.GSTPercent,
                    IsActive = p.IsActive,
                    ClosingStockValue = p.ClosingStockValue
                })
                .ToListAsync();

            _cache.Set(cacheKey, products, TimeSpan.FromMinutes(5));
            return products;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var cacheKey = $"{CacheKeys.Product}{id}";

            if (_cache.TryGetValue(cacheKey, out ProductDto? cached)
                && cached != null)
            {
                _logger.LogInformation(
                    "Product {Id} retrieved from cache", id);
                return cached;
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new NotFoundException(
                    $"Product with id {id} not found");

            var dto = MapToDto(product);
            _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));
            return dto;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var tenantId = _tenantService.GetTenantId();

            var product = new Product
            {
                TenantId = tenantId,
                Name = dto.Name,
                SKU = dto.SKU,
                HsnCode = dto.HsnCode,
                Description = dto.Description,
                PurchasePrice = dto.PurchasePrice,
                MarketValue = dto.MarketValue > 0
                    ? dto.MarketValue : dto.PurchasePrice,
                Price = dto.Price,
                Stock = dto.Stock,
                Unit = dto.Unit,
                GSTPercent = dto.GSTPercent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            InvalidateCache(tenantId);

            _logger.LogInformation(
                "Product {Name} created for tenant {TenantId}",
                dto.Name, tenantId);

            return MapToDto(product);
        }

        public async Task<bool> UpdateAsync(
            int id, UpdateProductDto dto)
        {
            var tenantId = _tenantService.GetTenantId();

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new NotFoundException(
                    $"Product with id {id} not found");

            product.Name = dto.Name;
            product.SKU = dto.SKU;
            product.HsnCode = dto.HsnCode;
            product.Description = dto.Description;
            product.PurchasePrice = dto.PurchasePrice;
            product.MarketValue = dto.MarketValue > 0
                ? dto.MarketValue : dto.PurchasePrice;
            product.Price = dto.Price;
            product.Unit = dto.Unit;
            product.GSTPercent = dto.GSTPercent;
            product.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            InvalidateCache(tenantId, id);

            _logger.LogInformation("Product {Id} updated", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tenantId = _tenantService.GetTenantId();

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new NotFoundException(
                    $"Product with id {id} not found");

            // Soft delete
            product.IsActive = false;
            await _context.SaveChangesAsync();

            InvalidateCache(tenantId, id);

            _logger.LogInformation("Product {Id} deleted", id);
            return true;
        }

        private void InvalidateCache(int tenantId, int? productId = null)
        {
            _cache.Remove($"{CacheKeys.AllProducts}_{tenantId}");
            if (productId.HasValue)
                _cache.Remove($"{CacheKeys.Product}{productId}");
        }

        private static ProductDto MapToDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            SKU = p.SKU,
            HsnCode = p.HsnCode,
            Description = p.Description,
            PurchasePrice = p.PurchasePrice,
            MarketValue = p.MarketValue,
            Price = p.Price,
            Stock = p.Stock,
            Unit = p.Unit,
            GSTPercent = p.GSTPercent,
            IsActive = p.IsActive,
            ClosingStockValue = p.ClosingStockValue
        };
    }
}