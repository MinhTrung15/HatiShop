// Repositories/CustomerRepository.cs
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
            return await _context.Customer
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(string id)
        {
            return await _context.Customer
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            return await _context.Customer
                .FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customer
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetByPhoneAsync(string phone)
        {
            return await _context.Customer
                .FirstOrDefaultAsync(c => c.PhoneNumber == phone);
        }

        public async Task<IEnumerable<Customer>> SearchByNameAsync(string name)
        {
            return await _context.Customer
                .Where(c => c.FullName.Contains(name))
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchByIdAsync(string id)
        {
            return await _context.Customer
                .Where(c => c.Id.Contains(id))
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchByPhoneAsync(string phone)
        {
            return await _context.Customer
                .Where(c => c.PhoneNumber != null && c.PhoneNumber.Contains(phone))
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(Customer customer)
        {
            try
            {
                await _context.Customer.AddAsync(customer);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating customer: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _context.Customer.FindAsync(customer.Id);
                if (existingCustomer == null)
                    return false;

                // Cập nhật từng trường
                _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
                _context.Entry(existingCustomer).State = EntityState.Modified;

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var customer = await GetByIdAsync(id);
                if (customer == null) return false;

                _context.Customer.Remove(customer);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Customer
                .AnyAsync(c => c.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Customer
                .AnyAsync(c => c.Email == email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Customer
                .AnyAsync(c => c.PhoneNumber == phone);
        }
    }
}