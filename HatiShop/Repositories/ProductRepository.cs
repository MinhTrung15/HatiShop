// Repositories/ProductRepository.cs
using Microsoft.EntityFrameworkCore;
using HatiShop.Models;
using HatiShop.Data;

namespace HatiShop.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Product
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            return await _context.Product
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            return await _context.Product
                .Where(p => p.Name.Contains(name))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchByIdAsync(string id)
        {
            return await _context.Product
                .Where(p => p.Id.Contains(id))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchByTypeAsync(string type)
        {
            return await _context.Product
                .Where(p => p.Type == type)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(Product product)
        {
            try
            {
                await _context.Product.AddAsync(product);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Product.FindAsync(product.Id);
                if (existingProduct == null)
                    return false;

                _context.Entry(existingProduct).CurrentValues.SetValues(product);
                _context.Entry(existingProduct).State = EntityState.Modified;

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var product = await GetByIdAsync(id);
                if (product == null) return false;

                _context.Product.Remove(product);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                return false;
            }
        }
    }
}