// Repositories/StaffRepository.cs
using Microsoft.EntityFrameworkCore;
using HatiShop.Models;
using HatiShop.Data;

namespace HatiShop.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _context.Staff
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }

        public async Task<Staff?> GetByIdAsync(string id)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Staff?> GetByUsernameAsync(string username)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.Username == username);
        }

        public async Task<Staff?> GetByEmailAsync(string email)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<Staff?> GetByPhoneAsync(string phone)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.PhoneNumber == phone);
        }

        public async Task<bool> CreateAsync(Staff staff)
        {
            try
            {
                await _context.Staff.AddAsync(staff);
                return await SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating staff: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Staff staff)
        {
            try
            {
                _context.Staff.Update(staff);
                return await SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating staff: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var staff = await GetByIdAsync(id);
                if (staff == null) return false;

                _context.Staff.Remove(staff);
                return await SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting staff: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Staff
                .AnyAsync(s => s.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Staff
                .AnyAsync(s => s.Email == email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Staff
                .AnyAsync(s => s.PhoneNumber == phone);
        }

        public async Task<IEnumerable<Staff>> SearchByNameAsync(string name)
        {
            return await _context.Staff
                .Where(s => s.FullName.Contains(name))
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Staff>> SearchByIdAsync(string id)
        {
            return await _context.Staff
                .Where(s => s.Id.Contains(id))
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Staff>> SearchByPhoneAsync(string phone)
        {
            return await _context.Staff
                .Where(s => s.PhoneNumber != null && s.PhoneNumber.Contains(phone))
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Staff>> SearchByRoleAsync(string role)
        {
            return await _context.Staff
                .Where(s => s.Role == role)
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }

        public async Task<Staff?> LoginAsync(string username, string password)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.Username == username && s.Password == password);
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