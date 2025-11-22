// Repositories/IBillRepository.cs
using HatiShop.Models;

namespace HatiShop.Repositories
{
    public interface IBillRepository
    {
        Task<Bill?> GetByIdAsync(string id);
        Task<IEnumerable<Bill>> GetAllAsync();
        Task<IEnumerable<Bill>> SearchAsync(string searchType, string searchValue);
        Task<bool> CreateAsync(Bill bill);
        Task<bool> UpdateAsync(Bill bill);
        Task<bool> DeleteAsync(string id);
        Task<bool> CreateBillDetailAsync(BillDetail billDetail);
        Task<IEnumerable<BillDetail>> GetBillDetailsAsync(string billId);
        Task<bool> UpdateBillDetailAsync(BillDetail billDetail);
        Task<bool> DeleteBillDetailAsync(string billId, string productId);
        Task<bool> PayBillAsync(string billId, decimal originalPrice, decimal discountAmount, decimal discountedTotal);
        Task<bool> UpdateCustomerRevenueAsync(string customerId, int discountedTotal);
    }
}