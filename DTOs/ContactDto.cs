using System.ComponentModel.DataAnnotations;
using TaxAccount.Models;

namespace TaxAccount.DTOs
{
    public class CreateContactDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Gstin { get; set; }

        public GstType GstType { get; set; } = GstType.Unregistered;

        public ContactType ContactType { get; set; } = ContactType.Customer;

        [StringLength(20)]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? PinCode { get; set; }

        public decimal OpeningBalance { get; set; } = 0;
    }

    public class UpdateContactDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Gstin { get; set; }

        public GstType GstType { get; set; } = GstType.Unregistered;

        public ContactType ContactType { get; set; } = ContactType.Customer;

        [StringLength(20)]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? PinCode { get; set; }

        public decimal OpeningBalance { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class ContactResponseDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Gstin { get; set; }
        public string GstType { get; set; } = string.Empty;
        public string ContactType { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ContactListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Gstin { get; set; }
        public string GstType { get; set; } = string.Empty;
        public string ContactType { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}