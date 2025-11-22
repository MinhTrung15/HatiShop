using Microsoft.EntityFrameworkCore;
using HatiShop.Models;
using HatiShop.Data;

namespace HatiShop.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(string id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetByPhoneAsync(string phone)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.PhoneNumber == phone);
        }

        public async Task<IEnumerable<Customer>> SearchByNameAsync(string name)
        {
            return await _context.Customers
                .Where(c => c.FullName.Contains(name))
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchByIdAsync(string id)
        {
            return await _context.Customers
                .Where(c => c.Id.Contains(id))
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchByPhoneAsync(string phone)
        {
            return await _context.Customers
                .Where(c => c.PhoneNumber != null && c.PhoneNumber.Contains(phone))
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(Customer customer)
        {
            try
            {
                await _context.Customers.AddAsync(customer);
                return await SaveAsync();
            }
            catch (Exception ex)
            {
                // Log error here if needed
                Console.WriteLine($"Error creating customer: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            try
            {
                _context.Customers.Update(customer);
                return await SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var customer = await GetByIdAsync(id);
                if (customer == null) return false;

                _context.Customers.Remove(customer);
                return await SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Customers
                .AnyAsync(c => c.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Customers
                .AnyAsync(c => c.Email == email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Customers
                .AnyAsync(c => c.PhoneNumber == phone);
        }

        private async Task<bool> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                return false;
            }
        }
    }
}