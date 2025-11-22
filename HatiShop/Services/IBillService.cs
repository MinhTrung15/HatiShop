// Services/IBillService.cs
using HatiShop.Models;

namespace HatiShop.Services
{
    public interface IBillService
    {
        Task<ServiceResult> CreateBillAsync(Bill bill);
        Task<ServiceResult> UpdateBillAsync(Bill bill);
        Task<ServiceResult> DeleteBillAsync(string id);
        Task<Bill?> GetBillByIdAsync(string id);
        Task<IEnumerable<Bill>> GetAllBillsAsync();
        Task<IEnumerable<Bill>> SearchBillsAsync(string searchType, string searchValue);
        Task<ServiceResult> AddBillDetailAsync(BillDetail billDetail);
        Task<ServiceResult> UpdateBillDetailAsync(BillDetail billDetail);
        Task<ServiceResult> RemoveBillDetailAsync(string billId, string productId);
        Task<ServiceResult> PayBillAsync(string billId, decimal originalPrice, decimal discountAmount, decimal discountedTotal);
        Task<IEnumerable<BillDetail>> GetBillDetailsAsync(string billId);
    }
}