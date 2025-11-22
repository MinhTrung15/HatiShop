// Repositories/IStaffRepository.cs
using HatiShop.Models;

namespace HatiShop.Repositories
{
    public interface IStaffRepository
    {
        Task<Staff?> GetByIdAsync(string id);
        Task<Staff?> GetByUsernameAsync(string username);
        Task<Staff?> GetByEmailAsync(string email);
        Task<Staff?> GetByPhoneAsync(string phone);
        Task<IEnumerable<Staff>> GetAllAsync();
        Task<bool> CreateAsync(Staff staff);
        Task<bool> UpdateAsync(Staff staff);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<Staff>> SearchByNameAsync(string name);
        Task<IEnumerable<Staff>> SearchByIdAsync(string id);
        Task<IEnumerable<Staff>> SearchByPhoneAsync(string phone);
        Task<IEnumerable<Staff>> SearchByRoleAsync(string role);
        Task<Staff?> LoginAsync(string username, string password);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phone);
    }
}