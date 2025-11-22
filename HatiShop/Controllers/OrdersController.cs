using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HatiShop.Models;
using HatiShop.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HatiShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var bills = _context.Bill
                .Include(b => b.Customer)
                .Include(b => b.Staff)
                .OrderByDescending(b => b.CreationTime);
            return View(await bills.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .Include(b => b.Customer)
                .Include(b => b.Staff)
                .Include(b => b.BillDetails)
                    .ThenInclude(bd => bd.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "FullName");
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "FullName");
            ViewData["Products"] = _context.Product.ToList();
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bill bill, List<BillDetail> billDetails)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(bill.Id))
                    {
                        bill.Id = "BILL_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    }

                    bill.CreationTime = DateTime.Now;

                    double originalPrice = 0;

                    foreach (var detail in billDetails)
                    {
                        var product = await _context.Product.FindAsync(detail.ProductId);
                        if (product != null)
                        {
                            // 🟢 XÓA CAST VÌ CÙNG KIỂU double
                            double itemTotal = product.Price * detail.Quantity;
                            detail.Total = itemTotal;
                            originalPrice += itemTotal;
                        }
                    }

                    bill.OriginalPrice = originalPrice;
                    bill.DiscountedTotal = originalPrice - bill.DiscountAmount;

                    _context.Bill.Add(bill);

                    foreach (var detail in billDetails)
                    {
                        detail.Id = bill.Id;
                        _context.BillDetail.Add(detail);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi tạo đơn hàng: " + ex.Message);
                }
            }

            ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "FullName", bill.CustomerId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "FullName", bill.StaffId);
            ViewData["Products"] = await _context.Product.ToListAsync();
            return View(bill);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .Include(b => b.BillDetails)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "FullName", bill.CustomerId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "FullName", bill.StaffId);
            ViewData["Products"] = _context.Product.ToList();
            return View(bill);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Bill bill, List<BillDetail> billDetails)
        {
            if (id != bill.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật bill
                    _context.Bill.Update(bill);

                    // Xóa bill details cũ và thêm mới
                    var existingDetails = _context.BillDetail.Where(bd => bd.Id == id);
                    _context.BillDetail.RemoveRange(existingDetails);

                    foreach (var detail in billDetails)
                    {
                        detail.Id = bill.Id;
                        _context.BillDetail.Add(detail);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "FullName", bill.CustomerId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "FullName", bill.StaffId);
            ViewData["Products"] = _context.Product.ToList();
            return View(bill);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .Include(b => b.Customer)
                .Include(b => b.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bill = await _context.Bill.FindAsync(id);
            if (bill != null)
            {
                // Xóa bill details trước
                var billDetails = _context.BillDetail.Where(bd => bd.Id == id);
                _context.BillDetail.RemoveRange(billDetails);

                // Xóa bill
                _context.Bill.Remove(bill);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Action thanh toán
        public async Task<IActionResult> Payment(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái thanh toán (nếu có field trạng thái)
            // bill.Status = "Paid";
            // _context.Update(bill);
            // await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool BillExists(string id)
        {
            return _context.Bill.Any(e => e.Id == id);
        }
    }
}