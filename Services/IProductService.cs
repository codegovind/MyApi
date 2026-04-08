using MyApi.DTOs;
using MyApi.Models;

namespace MyApi.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto productDto);
        Task<bool> UpdateAsync(int id, UpdateProductDto productDto);
        Task<bool> DeleteAsync(int id);
    }   
}