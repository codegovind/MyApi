using Microsoft.AspNetCore.Mvc;
using TaxAccount.Authorization;
using TaxAccount.DTOs;
using TaxAccount.Services;

namespace TaxAccount.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        [HasPermission("contacts.manage")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? type = null,
            [FromQuery] bool includeDefault = false)
        {
            var contacts = await _contactService
                .GetAllAsync(type, includeDefault);
            return Ok(contacts);
        }

        [HttpGet("vendors")]
        [HasPermission("invoices.view")]
        public async Task<IActionResult> GetVendors()
        {
            var vendors = await _contactService.GetVendorsAsync();
            return Ok(vendors);
        }

        [HttpGet("customers")]
        [HasPermission("invoices.view")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _contactService.GetCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        [HasPermission("contacts.manage")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            return Ok(contact);
        }

        [HttpPost]
        [HasPermission("contacts.manage")]
        public async Task<IActionResult> Create(CreateContactDto dto)
        {
            var contact = await _contactService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = contact.Id },
                contact);
        }

        [HttpPut("{id}")]
        [HasPermission("contacts.manage")]
        public async Task<IActionResult> Update(
            int id, UpdateContactDto dto)
        {
            var contact = await _contactService.UpdateAsync(id, dto);
            return Ok(contact);
        }

        [HttpDelete("{id}")]
        [HasPermission("contacts.manage")]
        public async Task<IActionResult> Delete(int id)
        {
            await _contactService.DeleteAsync(id);
            return NoContent();
        }
    }
}