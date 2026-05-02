using System.ComponentModel.DataAnnotations;

namespace TaxAccount.DTOs
{
    public class RegisterDto
    {
        // Company Info
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company email is required")]
        [EmailAddress]
        public string CompanyEmail { get; set; } = string.Empty;

        // Owner Info
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        public int RoleId { get; set; } = 1;
    }
}