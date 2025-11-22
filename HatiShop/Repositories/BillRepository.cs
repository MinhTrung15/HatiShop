using Microsoft.EntityFrameworkCore;
using HatiShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HatiShop.Data;

namespace HatiShop.Repositories
{
    public class BillRepository
    {
        private readonly ApplicationDbContext _context;

        public BillRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Bill> GetBillByIdAsync(string id) // ✅ ĐỔI THÀNH string
        {
            return await _context.Bill
                .Include(b => b.Customer)
                .Include(b => b.Staff)
                .Include(b => b.BillDetails) // ✅ SỬA OrderItems → BillDetails
                    .ThenInclude(bd => bd.Product)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Bill>> GetAllBillsAsync()
        {
            return await _context.Bill
                .Include(b => b.Customer)
                .Include(b => b.Staff)
                .Include(b => b.BillDetails) // ✅ SỬA OrderItems → BillDetails
                    .ThenInclude(bd => bd.Product)
                .ToListAsync();
        }

        public async Task<List<Bill>> GetBillsByCustomerIdAsync(string customerId) // ✅ ĐỔI THÀNH string
        {
            return await _context.Bill
                .Include(b => b.Customer)
                .Include(b => b.Staff)
                .Include(b => b.BillDetails) // ✅ SỬA OrderItems → BillDetails
                .Where(b => b.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<List<Bill>> SearchBillsAsync(string searchString)
        {
            var query = _context.Bill
                .Include(b => b.Customer)
                .Include(b => b.Staff)
                .Include(b => b.BillDetails) // ✅ SỬA OrderItems → BillDetails
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b =>
                    (b.Customer != null && b.Customer.FullName.Contains(searchString)) || // ✅ SỬA Name → FullName
                    b.Id.Contains(searchString) ||
                    (b.Staff != null && b.Staff.FullName.Contains(searchString)) || // ✅ SỬA Name → FullName
                    (b.Customer != null && b.Customer.Email.Contains(searchString))
                );
            }

            return await query.ToListAsync();
        }

        // Các method khác...
        public async Task AddBillAsync(Bill bill)
        {
            await _context.Bill.AddAsync(bill);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBillAsync(Bill bill)
        {
            _context.Bill.Update(bill);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBillAsync(string id) // ✅ ĐỔI THÀNH string
        {
            var bill = await GetBillByIdAsync(id);
            if (bill != null)
            {
                _context.Bill.Remove(bill);
                await _context.SaveChangesAsync();
            }
        }
    }
}