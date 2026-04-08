using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    // public ProductsController(AppDbContext context)
    // {
    //     _context = context;
    // }


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto product)
    {
        var createdProduct = await _productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new {id = createdProduct.Id}, createdProduct);
    }

    [HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var product = await _productService.GetByIdAsync(id);

    if (product == null)
        return NotFound();

    return Ok(product);
}
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, UpdateProductDto product)
{
    var updated = await _productService.UpdateAsync(id,product);

    if(!updated)
    return NotFound();

    return NoContent();
}
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var deleted = await _productService.DeleteAsync(id);

    if (!deleted)
        return NotFound();

    return NoContent();
}
}