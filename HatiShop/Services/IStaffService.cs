// Services/IStaffService.cs
using HatiShop.Models;

namespace HatiShop.Services
{
    public interface IStaffService
    {
        Task<ServiceResult> CreateStaffAsync(Staff staff, IFormFile? avatarFile);
        Task<ServiceResult> UpdateStaffAsync(Staff staff, IFormFile? avatarFile);
        Task<ServiceResult> DeleteStaffAsync(string id);
        Task<Staff?> GetStaffByIdAsync(string id);
        Task<IEnumerable<Staff>> GetAllStaffAsync();
        Task<IEnumerable<Staff>> SearchStaffAsync(string searchType, string searchValue);
        Task<ServiceResult> LoginAsync(string username, string password);
    }
}