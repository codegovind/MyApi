using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaxAccount.Authorization;
using TaxAccount.DTOs;
using TaxAccount.Services;

namespace TaxAccount.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        private int GetUserId() => int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // ── Bills ──

        [HttpGet("bills")]
        [HasPermission("invoices.view")]
        public async Task<IActionResult> GetAllBills()
        {
            var bills = await _purchaseService.GetAllBillsAsync();
            return Ok(bills);
        }

        [HttpGet("bills/{id}")]
        [HasPermission("invoices.view")]
        public async Task<IActionResult> GetBillById(int id)
        {
            var bill = await _purchaseService.GetBillByIdAsync(id);
            return Ok(bill);
        }

        [HttpPost("bills")]
        [HasPermission("invoices.create")]
        public async Task<IActionResult> CreateBill(
            CreatePurchaseBillDto dto)
        {
            var bill = await _purchaseService
                .CreateBillAsync(dto, GetUserId());
            return CreatedAtAction(
                nameof(GetBillById), new { id = bill.Id }, bill);
        }

        [HttpDelete("bills/{id}")]
        [HasPermission("invoices.approve")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            await _purchaseService.DeleteBillAsync(id);
            return NoContent();
        }

        // ── Orders ──

        [HttpGet("orders")]
        [HasPermission("invoices.view")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _purchaseService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("orders/{id}")]
        [HasPermission("invoices.view")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _purchaseService.GetOrderByIdAsync(id);
            return Ok(order);
        }

        [HttpPost("orders")]
        [HasPermission("invoices.create")]
        public async Task<IActionResult> CreateOrder(
            CreatePurchaseOrderDto dto)
        {
            var order = await _purchaseService
                .CreateOrderAsync(dto, GetUserId());
            return CreatedAtAction(
                nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpPatch("orders/{id}/status")]
        [HasPermission("invoices.approve")]
        public async Task<IActionResult> UpdateOrderStatus(
            int id, UpdatePurchaseOrderStatusDto dto)
        {
            var order = await _purchaseService
                .UpdateOrderStatusAsync(id, dto);
            return Ok(order);
        }

        [HttpPost("orders/{id}/convert-to-bill")]
        [HasPermission("invoices.create")]
        public async Task<IActionResult> ConvertOrderToBill(int id)
        {
            var bill = await _purchaseService
                .ConvertOrderToBillAsync(id, GetUserId());
            return Ok(bill);
        }
    }
}