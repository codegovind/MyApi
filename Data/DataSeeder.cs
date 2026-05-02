using TaxAccount.Models;

namespace TaxAccount.Data
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(AppDbContext context, ILogger<DataSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Called after every new tenant registration
        public async Task SeedTenantDefaultsAsync(int tenantId)
        {
            await CreateDefaultCashContactAsync(tenantId);
            _logger.LogInformation(
                "Default data seeded for tenant {TenantId}", tenantId);
        }

        private async Task CreateDefaultCashContactAsync(int tenantId)
        {
            // Create default Cash Customer for counter sales
            var cashCustomer = new Contact
            {
                TenantId = tenantId,
                Name = "Cash Customer",
                GstType = GstType.Consumer,
                ContactType = ContactType.Both,
                IsDefault = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Contacts.Add(cashCustomer);
            await _context.SaveChangesAsync();
        }
    }
}