using Microsoft.EntityFrameworkCore;
using TaxAccount.Data;
using TaxAccount.DTOs;
using TaxAccount.Exceptions;
using TaxAccount.Models;

namespace TaxAccount.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(AppDbContext context, ILogger<InvoiceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<InvoiceResponseDto>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.CreatedBy)
                .Include(i => i.Items)
                    .ThenInclude(item => item.Product)
                .Select(i => MapToResponseDto(i))
                .ToListAsync();
        }

        public async Task<InvoiceResponseDto> GetByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.CreatedBy)
                .Include(i => i.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                throw new NotFoundException($"Invoice with id {id} not found");

            return MapToResponseDto(invoice);
        }

        public async Task<InvoiceResponseDto> CreateAsync(
            CreateInvoiceDto dto, int createdByUserId)
        {
            // Verify customer exists
            var customer = await _context.Users.FindAsync(dto.CustomerId);
            if (customer == null)
                throw new NotFoundException("Customer not found");

            // Generate invoice number
            var invoiceNumber = await GenerateInvoiceNumberAsync();

            // Calculate totals
            var items = new List<InvoiceItem>();
            decimal subTotal = 0;
            decimal totalTax = 0;

            foreach (var itemDto in dto.Items)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product {itemDto.ProductId} not found");

                var itemSubTotal = itemDto.Quantity * itemDto.UnitPrice;
                var itemTax = itemSubTotal * (itemDto.TaxPercent / 100);
                var itemTotal = itemSubTotal + itemTax;

                items.Add(new InvoiceItem
                {
                    ProductId = itemDto.ProductId,
                    Description = itemDto.Description,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TaxPercent = itemDto.TaxPercent,
                    TaxAmount = itemTax,
                    TotalAmount = itemTotal
                });

                subTotal += itemSubTotal;
                totalTax += itemTax;
            }

            var invoice = new Invoice
            {
                InvoiceNumber = invoiceNumber,
                InvoiceDate = DateTime.UtcNow,
                DueDate = dto.DueDate,
                Status = InvoiceStatus.Draft,
                CustomerId = dto.CustomerId,
                CreatedByUserId = createdByUserId,
                Notes = dto.Notes,
                SubTotal = subTotal,
                TaxAmount = totalTax,
                TotalAmount = subTotal + totalTax,
                Items = items
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Invoice {InvoiceNumber} created by user {UserId}",
                invoiceNumber, createdByUserId);

            return await GetByIdAsync(invoice.Id);
        }

        public async Task<InvoiceResponseDto> UpdateStatusAsync(
            int id, UpdateInvoiceStatusDto dto)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                throw new NotFoundException($"Invoice with id {id} not found");

            // Prevent invalid status changes
            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new AppException("Cannot update a cancelled invoice");

            invoice.Status = dto.Status;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Invoice {Id} status updated to {Status}", id, dto.Status);

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                throw new NotFoundException($"Invoice with id {id} not found");

            if (invoice.Status != InvoiceStatus.Draft)
                throw new AppException("Only draft invoices can be deleted");

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Invoice {Id} deleted", id);
            return true;
        }

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var count = await _context.Invoices
                .CountAsync(i => i.InvoiceDate.Year == year);

            return $"INV-{year}-{(count + 1):D4}";
        }

        private static InvoiceResponseDto MapToResponseDto(Invoice i)
        {
            return new InvoiceResponseDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = i.InvoiceDate,
                DueDate = i.DueDate,
                Status = i.Status.ToString(),
                CustomerId = i.CustomerId,
                CustomerName = $"{i.Customer.FirstName} {i.Customer.LastName}",
                CreatedByName = $"{i.CreatedBy.FirstName} {i.CreatedBy.LastName}",
                Notes = i.Notes,
                SubTotal = i.SubTotal,
                TaxAmount = i.TaxAmount,
                TotalAmount = i.TotalAmount,
                CreatedAt = i.CreatedAt,
                Items = i.Items.Select(item => new InvoiceItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TaxPercent = item.TaxPercent,
                    TaxAmount = item.TaxAmount,
                    TotalAmount = item.TotalAmount
                }).ToList()
            };
        }
    }
}