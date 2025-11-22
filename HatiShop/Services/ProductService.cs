// Services/ProductService.cs
using HatiShop.Models;
using HatiShop.Repositories;
using HatiShop.Data;

namespace HatiShop.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _environment;

        public ProductService(IProductRepository productRepository, IWebHostEnvironment environment)
        {
            _productRepository = productRepository;
            _environment = environment;
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchType, string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
                return await _productRepository.GetAllAsync();

            return searchType?.ToLower() switch
            {
                "name" => await _productRepository.SearchByNameAsync(searchValue),
                "id" => await _productRepository.SearchByIdAsync(searchValue),
                "type" => await _productRepository.SearchByTypeAsync(searchValue),
                _ => await _productRepository.GetAllAsync()
            };
        }

        public async Task<ServiceResult> CreateProductAsync(Product product, IFormFile? imageFile)
        {
            try
            {
                // Check if product ID already exists
                if (await _productRepository.GetByIdAsync(product.Id) != null)
                    return new ServiceResult { Success = false, Message = "Mã sản phẩm đã tồn tại" };

                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    product.AvatarPath = await SaveProductImageAsync(imageFile);
                }

                var result = await _productRepository.CreateAsync(product);

                return new ServiceResult
                {
                    Success = result,
                    Message = result ? "Tạo sản phẩm thành công" : "Tạo sản phẩm thất bại"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> UpdateProductAsync(Product product, IFormFile? imageFile)
        {
            try
            {
                var existingProduct = await _productRepository.GetByIdAsync(product.Id);
                if (existingProduct == null)
                    return new ServiceResult { Success = false, Message = "Không tìm thấy sản phẩm" };

                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingProduct.AvatarPath))
                    {
                        DeleteProductImage(existingProduct.AvatarPath);
                    }
                    product.AvatarPath = await SaveProductImageAsync(imageFile);
                }
                else
                {
                    product.AvatarPath = existingProduct.AvatarPath;
                }

                var result = await _productRepository.UpdateAsync(product);

                return new ServiceResult
                {
                    Success = result,
                    Message = result ? "Cập nhật sản phẩm thành công" : "Cập nhật sản phẩm thất bại"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> DeleteProductAsync(string id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return new ServiceResult { Success = false, Message = "Không tìm thấy sản phẩm" };

                if (!string.IsNullOrEmpty(product.AvatarPath))
                {
                    DeleteProductImage(product.AvatarPath);
                }

                var result = await _productRepository.DeleteAsync(id);
                return new ServiceResult
                {
                    Success = result,
                    Message = result ? "Xóa sản phẩm thành công" : "Xóa sản phẩm thất bại"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<string> SaveProductImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/images/products/{uniqueFileName}";
        }

        private void DeleteProductImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}