using Microsoft.EntityFrameworkCore;
using TaxAccount.Data;
using TaxAccount.DTOs;
using TaxAccount.Exceptions;
using TaxAccount.Models;

namespace TaxAccount.Services
{
    public class ContactService : IContactService
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            AppDbContext context,
            ITenantService tenantService,
            ILogger<ContactService> logger)
        {
            _context = context;
            _tenantService = tenantService;
            _logger = logger;
        }

        public async Task<List<ContactListDto>> GetAllAsync(
            string? contactType = null,
            bool includeDefault = false)
        {
            var query = _context.Contacts
                .Where(c => c.IsActive);

            if (!includeDefault)
                query = query.Where(c => !c.IsDefault);

            if (!string.IsNullOrEmpty(contactType))
            {
                if (Enum.TryParse<ContactType>(
                    contactType, true, out var ct))
                {
                    query = query.Where(c =>
                        c.ContactType == ct ||
                        c.ContactType == ContactType.Both);
                }
            }

            return await query
                .OrderBy(c => c.Name)
                .Select(c => new ContactListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Gstin = c.Gstin,
                    GstType = c.GstType.ToString(),
                    ContactType = c.ContactType.ToString(),
                    Phone = c.Phone,
                    City = c.City,
                    State = c.State,
                    OpeningBalance = c.OpeningBalance,
                    IsDefault = c.IsDefault,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<ContactResponseDto> GetByIdAsync(int id)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                throw new NotFoundException(
                    $"Contact with id {id} not found");

            return MapToResponseDto(contact);
        }

        public async Task<ContactResponseDto> CreateAsync(
            CreateContactDto dto)
        {
            var tenantId = _tenantService.GetTenantId();

            // Validate GSTIN if provided
            if (!string.IsNullOrEmpty(dto.Gstin))
            {
                var existingGstin = await _context.Contacts
                    .AnyAsync(c => c.Gstin == dto.Gstin);

                if (existingGstin)
                    throw new AppException(
                        "A contact with this GSTIN already exists", 409);
            }

            var contact = new Contact
            {
                TenantId = tenantId,
                Name = dto.Name,
                Gstin = string.IsNullOrEmpty(dto.Gstin)
                    ? null : dto.Gstin.ToUpper(),
                GstType = dto.GstType,
                ContactType = dto.ContactType,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                PinCode = dto.PinCode,
                OpeningBalance = dto.OpeningBalance,
                IsDefault = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Contact {Name} created for tenant {TenantId}",
                dto.Name, tenantId);

            return MapToResponseDto(contact);
        }

        public async Task<ContactResponseDto> UpdateAsync(
            int id, UpdateContactDto dto)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                throw new NotFoundException(
                    $"Contact with id {id} not found");

            if (contact.IsDefault)
                throw new AppException(
                    "Default cash contact cannot be modified");

            // Validate GSTIN uniqueness if changed
            if (!string.IsNullOrEmpty(dto.Gstin) &&
                dto.Gstin != contact.Gstin)
            {
                var existingGstin = await _context.Contacts
                    .AnyAsync(c =>
                        c.Gstin == dto.Gstin && c.Id != id);

                if (existingGstin)
                    throw new AppException(
                        "A contact with this GSTIN already exists", 409);
            }

            contact.Name = dto.Name;
            contact.Gstin = string.IsNullOrEmpty(dto.Gstin)
                ? null : dto.Gstin.ToUpper();
            contact.GstType = dto.GstType;
            contact.ContactType = dto.ContactType;
            contact.Phone = dto.Phone;
            contact.Address = dto.Address;
            contact.City = dto.City;
            contact.State = dto.State;
            contact.PinCode = dto.PinCode;
            contact.OpeningBalance = dto.OpeningBalance;
            contact.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Contact {Id} updated", id);

            return MapToResponseDto(contact);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                throw new NotFoundException(
                    $"Contact with id {id} not found");

            if (contact.IsDefault)
                throw new AppException(
                    "Default cash contact cannot be deleted");

            // Check if contact has invoices
            var hasInvoices = await _context.Invoices
                .AnyAsync(i => i.ContactId == id);

            if (hasInvoices)
                throw new AppException(
                    "Cannot delete contact with existing invoices. " +
                    "Deactivate instead.", 409);

            // Check if contact has purchase orders
            var hasPurchaseOrders = await _context.PurchaseOrders
                .AnyAsync(po => po.ContactId == id);

            if (hasPurchaseOrders)
                throw new AppException(
                    "Cannot delete contact with existing purchase orders. " +
                    "Deactivate instead.", 409);

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Contact {Id} deleted", id);
            return true;
        }

        public async Task<List<ContactListDto>> GetVendorsAsync()
        {
            return await GetAllAsync("Vendor");
        }

        public async Task<List<ContactListDto>> GetCustomersAsync()
        {
            return await GetAllAsync("Customer");
        }

        private static ContactResponseDto MapToResponseDto(
            Contact c) => new()
        {
            Id = c.Id,
            TenantId = c.TenantId,
            Name = c.Name,
            Gstin = c.Gstin,
            GstType = c.GstType.ToString(),
            ContactType = c.ContactType.ToString(),
            Phone = c.Phone,
            Address = c.Address,
            City = c.City,
            State = c.State,
            PinCode = c.PinCode,
            OpeningBalance = c.OpeningBalance,
            IsDefault = c.IsDefault,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };
    }
}