// Services/ICustomerService.cs
using HatiShop.Models;

namespace HatiShop.Services
{
    public interface ICustomerService
    {
        Task<ServiceResult> CreateCustomerAsync(Customer customer, IFormFile? avatarFile);
        Task<ServiceResult> UpdateCustomerAsync(Customer customer, IFormFile? avatarFile);
        Task<ServiceResult> DeleteCustomerAsync(string id);
        Task<Customer?> GetCustomerByIdAsync(string id);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchType, string searchValue);
    }
}