namespace TaxAccount.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public List<string> Permissions { get; set; } = new();
        public DateTime ExpiresAt { get; set; }
    }
}