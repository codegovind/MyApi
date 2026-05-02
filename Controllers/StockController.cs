using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaxAccount.Authorization;
using TaxAccount.DTOs;
using TaxAccount.Services;

namespace TaxAccount.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost("adjust")]
        [HasPermission("products.edit")]
        public async Task<IActionResult> AdjustStock(
            CreateStockAdjustmentDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _stockService.AdjustStockAsync(dto, userId);
            return Ok(result);
        }

        [HttpGet("{productId}/adjustments")]
        [HasPermission("products.view")]
        public async Task<IActionResult> GetAdjustments(int productId)
        {
            var result = await _stockService.GetAdjustmentsAsync(productId);
            return Ok(result);
        }

        [HttpGet("{productId}/current")]
        [HasPermission("products.view")]
        public async Task<IActionResult> GetCurrentStock(int productId)
        {
            var stock = await _stockService.GetCurrentStockAsync(productId);
            return Ok(new { productId, currentStock = stock });
        }
    }
}