using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxAccount.Authorization;
using TaxAccount.Data;
using TaxAccount.Models;
using TaxAccount.Services;
using TaxAccount.DTOs;

namespace TaxAccount.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    //private readonly AppDbContext _context;
     private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [HasPermission("products.view")]
    public async Task<IActionResult> Get()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpPost]
    [HasPermission("products.create")]
    public async Task<IActionResult> Create(CreateProductDto product)
    {
        var createdProduct = await _productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new {id = createdProduct.Id}, createdProduct);
    }

    [HttpGet("{id}")]
    [HasPermission("products.view")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }
    
    [HttpPut("{id}")]
    [HasPermission("products.edit")]
    public async Task<IActionResult> Update(int id, UpdateProductDto product)
    {
        await _productService.UpdateAsync(id,product);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    [HasPermission("products.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}