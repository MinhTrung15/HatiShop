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
        Task<string> SaveAvatarAsync(IFormFile avatarFile);
    }

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}