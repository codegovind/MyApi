using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TaxAccount.Data;
using TaxAccount.Models;
using TaxAccount.DTOs;
using TaxAccount.Exceptions;
using TaxAccount.Helpers;

namespace TaxAccount.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AppDbContext context, IMemoryCache cache, ILogger<ProductService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            // Check cache first
            if (_cache.TryGetValue(CacheKeys.AllProducts, out List<ProductDto>? cached)
                && cached != null)
            {
                _logger.LogInformation("Products retrieved from cache");
                return cached;
            }

            // Cache miss - hit database
            _logger.LogInformation("Cache miss - fetching products from database");

            var products = await _context.Products
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })
            .ToListAsync();

            // Store in cache for 5 minutes
            _cache.Set(CacheKeys.AllProducts, products, TimeSpan.FromMinutes(5));

            return products;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var cacheKey = CacheKeys.Product + id;

            if (_cache.TryGetValue(cacheKey, out ProductDto? cached)
                && cached != null)
            {
                _logger.LogInformation("Product {Id} retrieved from cache", id);
                return cached;
            }

            _logger.LogInformation("Cache miss - fetching product {Id} from database", id);

            var product = await _context.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })
            .FirstOrDefaultAsync();

            if (product == null)
            throw new NotFoundException($"Product with id {id} not found");

            // Cache for 5 minutes
            _cache.Set(cacheKey, product, TimeSpan.FromMinutes(5));

            return product;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name ?? string.Empty,
                Price = dto.Price
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            // Invalidate all products cache
            _cache.Remove(CacheKeys.AllProducts);
            _logger.LogInformation("Product created, cache invalidated");


            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with id {id} not found");

            product.Name = updatedProduct.Name ?? string.Empty;
            product.Price = updatedProduct.Price;

            await _context.SaveChangesAsync();
            // Invalidate both caches
            _cache.Remove(CacheKeys.AllProducts);
            _cache.Remove(CacheKeys.Product + id);
            _logger.LogInformation("Product {Id} updated, cache invalidated", id);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with id {id} not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            // Invalidate both caches
            _cache.Remove(CacheKeys.AllProducts);
            _cache.Remove(CacheKeys.Product + id);
            _logger.LogInformation("Product {Id} deleted, cache invalidated", id);

            return true;
        }
    }
}