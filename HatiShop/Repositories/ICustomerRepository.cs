// Repositories/ICustomerRepository.cs
using HatiShop.Models;

namespace HatiShop.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task<Customer?> GetByUsernameAsync(string username);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetByPhoneAsync(string phone);
        Task<bool> CreateAsync(Customer customer);
        Task<bool> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(string id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phone);
        Task<IEnumerable<Customer>> SearchByNameAsync(string name);
        Task<IEnumerable<Customer>> SearchByIdAsync(string id);
        Task<IEnumerable<Customer>> SearchByPhoneAsync(string phone);
    }
}