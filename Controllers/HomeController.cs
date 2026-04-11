using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaxAccount.Authorization;
using TaxAccount.Data;

namespace TaxAccount.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        [HasPermission("reports.view")]
        public async Task<IActionResult> GetDashboard()
        {
            var totalInvoices = await _context.Invoices.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            var totalRevenue = await _context.Invoices
                .Where(i => i.Status == Models.InvoiceStatus.Paid)
                .SumAsync(i => i.TotalAmount);

            var pendingInvoices = await _context.Invoices
                .CountAsync(i => i.Status == Models.InvoiceStatus.Sent);

            var draftInvoices = await _context.Invoices
                .CountAsync(i => i.Status == Models.InvoiceStatus.Draft);

            var recentInvoices = await _context.Invoices
                .Include(i => i.Customer)
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    CustomerName = $"{i.Customer.FirstName} {i.Customer.LastName}",
                    i.TotalAmount,
                    Status = i.Status.ToString(),
                    i.InvoiceDate
                })
                .ToListAsync();

            return Ok(new
            {
                totalInvoices,
                totalProducts,
                totalUsers,
                totalRevenue,
                pendingInvoices,
                draftInvoices,
                recentInvoices
            });
        }
    }
}