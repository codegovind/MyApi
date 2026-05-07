using TaxAccount.DTOs;

namespace TaxAccount.Services
{
    public interface IContactService
    {
        Task<List<ContactListDto>> GetAllAsync(
            string? contactType = null,
            bool includeDefault = false);
        Task<ContactResponseDto> GetByIdAsync(int id);
        Task<ContactResponseDto> CreateAsync(CreateContactDto dto);
        Task<ContactResponseDto> UpdateAsync(int id, UpdateContactDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<ContactListDto>> GetVendorsAsync();
        Task<List<ContactListDto>> GetCustomersAsync();
    }
}