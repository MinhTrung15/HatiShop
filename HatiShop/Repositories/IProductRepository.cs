// Repositories/IProductRepository.cs
using HatiShop.Models;

namespace HatiShop.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(string id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<bool> CreateAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<Product>> SearchByNameAsync(string name);
        Task<IEnumerable<Product>> SearchByIdAsync(string id);
        Task<IEnumerable<Product>> SearchByTypeAsync(string type);
    }
}