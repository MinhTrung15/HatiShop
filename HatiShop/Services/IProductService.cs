// Services/IProductService.cs
using HatiShop.Models;

namespace HatiShop.Services
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(string id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchType, string searchValue);
        Task<ServiceResult> CreateProductAsync(Product product, IFormFile? imageFile);
        Task<ServiceResult> UpdateProductAsync(Product product, IFormFile? imageFile);
        Task<ServiceResult> DeleteProductAsync(string id);
    }
}