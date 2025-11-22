using HatiShop.Data;
using HatiShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HatiShop.Services
{
    public class BillService
    {
        private readonly ApplicationDbContext _context;

        public BillService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Bill> CreateBillAsync(Bill bill)
        {
            bill.CreationTime = DateTime.Now;
            _context.Bill.Add(bill);
            await _context.SaveChangesAsync();
            return bill;
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
                .ToListAsync();
        }

        public async Task<List<Bill>> GetBillsByCustomerAsync(string customerId) // ✅ ĐỔI THÀNH string
        {
            return await _context.Bill
                .Where(b => b.CustomerId == customerId) // ✅ ĐÃ SỬA: string == string
                .Include(b => b.Customer)
                .Include(b => b.Staff)
                .Include(b => b.BillDetails) // ✅ SỬA OrderItems → BillDetails
                .OrderByDescending(b => b.CreationTime)
                .ToListAsync();
        }

        public async Task UpdateBillAsync(Bill bill)
        {
            bill.DiscountedTotal = bill.OriginalPrice - bill.DiscountAmount;
            _context.Bill.Update(bill);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteBillAsync(string id) // ✅ ĐỔI THÀNH string
        {
            var bill = await GetBillByIdAsync(id);
            if (bill != null)
            {
                _context.Bill.Remove(bill);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<List<Bill>> SearchBillsAsync(string searchType, string searchValue)
        {
            var bills = await GetAllBillsAsync();

            return searchType?.ToLower() switch
            {
                "id" => bills.Where(b => b.Id.Contains(searchValue)).ToList(),
                "customer" => bills.Where(b => b.Customer != null && b.Customer.FullName.Contains(searchValue)).ToList(),
                "date" => bills.Where(b => b.CreationTime.ToString("dd/MM/yyyy").Contains(searchValue)).ToList(),
                _ => bills
            };
        }

        public double CalculateTotal(string billId) // ✅ ĐỔI THÀNH string
        {
            var bill = _context.Bill
                .Include(b => b.BillDetails) // ✅ SỬA OrderItems → BillDetails
                .FirstOrDefault(b => b.Id == billId);

            if (bill != null)
            {
                return bill.BillDetails.Sum(bd => bd.Total); // ✅ SỬA OrderItems → BillDetails
            }
            return 0;
        }

    }
}