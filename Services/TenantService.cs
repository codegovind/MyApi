using System.Security.Claims;

namespace TaxAccount.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetTenantId()
        {
            var tenantIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst("tenantId")?.Value;

            if (string.IsNullOrEmpty(tenantIdClaim))
                throw new UnauthorizedAccessException("Tenant not found in token");

            return int.Parse(tenantIdClaim);
        }
    }
}