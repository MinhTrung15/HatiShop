// Controllers/CustomersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HatiShop.Models;
using HatiShop.Services;

namespace HatiShop.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string searchType, string searchValue)
        {
            ViewBag.SearchTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "all", Text = "Tất cả" },
                new SelectListItem { Value = "name", Text = "Theo tên" },
                new SelectListItem { Value = "id", Text = "Theo mã" },
                new SelectListItem { Value = "phone", Text = "Theo SĐT" }
            };

            IEnumerable<Customer> customer;

            if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrEmpty(searchType) && searchType != "all")
            {
                customer = await _customerService.SearchCustomersAsync(searchType, searchValue);
                ViewBag.SearchType = searchType;
                ViewBag.SearchValue = searchValue;
            }
            else
            {
                customer = await _customerService.GetAllCustomersAsync();
            }

            return View(customer);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            // Khởi tạo model mới
            var customer = new Customer
            {
                Id = GenerateCustomerId(), // Tạo mã KH tự động
                Rank = "ĐỒNG", // Set giá trị mặc định
                Revenue = 0 // Set doanh thu mặc định
            };

            ViewBag.Genders = new List<SelectListItem>
    {
        new SelectListItem { Value = "Nam", Text = "Nam" },
        new SelectListItem { Value = "Nữ", Text = "Nữ" },
        new SelectListItem { Value = "Khác", Text = "Khác" }
    };

            ViewBag.Ranks = new List<SelectListItem>
    {
        new SelectListItem { Value = "MỚI", Text = "MỚI" },
        new SelectListItem { Value = "ĐỒNG", Text = "ĐỒNG", Selected = true },
        new SelectListItem { Value = "BẠC", Text = "BẠC" },
        new SelectListItem { Value = "VÀNG", Text = "VÀNG" },
        new SelectListItem { Value = "KIM CƯƠNG", Text = "KIM CƯƠNG" }
    };

            return View(customer);
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer, IFormFile? AvatarFile)
        {
            // Xóa validation cho file
            ModelState.Remove("AvatarFile");

            if (ModelState.IsValid)
            {
                var result = await _customerService.CreateCustomerAsync(customer, AvatarFile);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message);
            }

            // Reload dropdowns nếu có lỗi
            ReloadCreateViewBags(customer);

            return View(customer);
        }

        // Helper method để tạo mã KH
        private string GenerateCustomerId()
        {
            return "KH_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        // Helper method để reload dropdowns
        private void ReloadCreateViewBags(Customer customer)
        {
            ViewBag.Genders = new List<SelectListItem>
    {
        new SelectListItem { Value = "Nam", Text = "Nam", Selected = customer.Gender == "Nam" },
        new SelectListItem { Value = "Nữ", Text = "Nữ", Selected = customer.Gender == "Nữ" },
        new SelectListItem { Value = "Khác", Text = "Khác", Selected = customer.Gender == "Khác" }
    };

            ViewBag.Ranks = new List<SelectListItem>
    {
        new SelectListItem { Value = "MỚI", Text = "MỚI", Selected = customer.Rank == "MỚI" },
        new SelectListItem { Value = "ĐỒNG", Text = "ĐỒNG", Selected = customer.Rank == "ĐỒNG" },
        new SelectListItem { Value = "BẠC", Text = "BẠC", Selected = customer.Rank == "BẠC" },
        new SelectListItem { Value = "VÀNG", Text = "VÀNG", Selected = customer.Rank == "VÀNG" },
        new SelectListItem { Value = "KIM CƯƠNG", Text = "KIM CƯƠNG", Selected = customer.Rank == "KIM CƯƠNG" }
    };
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            ViewBag.Genders = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam", Selected = customer.Gender == "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ", Selected = customer.Gender == "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác", Selected = customer.Gender == "Khác" }
            };

            ViewBag.Ranks = new List<SelectListItem>
            {
                new SelectListItem { Value = "MỚI", Text = "MỚI", Selected = customer.Rank == "MỚI" },
                new SelectListItem { Value = "ĐỒNG", Text = "ĐỒNG", Selected = customer.Rank == "ĐỒNG" },
                new SelectListItem { Value = "BẠC", Text = "BẠC", Selected = customer.Rank == "BẠC" },
                new SelectListItem { Value = "VÀNG", Text = "VÀNG", Selected = customer.Rank == "VÀNG" },
                new SelectListItem { Value = "KIM CƯƠNG", Text = "KIM CƯƠNG", Selected = customer.Rank == "KIM CƯƠNG" }
            };

            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Customer customer, IFormFile? AvatarFile)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            // Xóa validation cho các trường không cần thiết
            ModelState.Remove("Id");
            ModelState.Remove("AvatarFile");

            if (ModelState.IsValid)
            {
                var result = await _customerService.UpdateCustomerAsync(customer, AvatarFile);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message);
            }

            // Reload dropdowns nếu có lỗi
            ViewBag.Genders = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam", Selected = customer.Gender == "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ", Selected = customer.Gender == "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác", Selected = customer.Gender == "Khác" }
            };

            ViewBag.Ranks = new List<SelectListItem>
            {
                new SelectListItem { Value = "MỚI", Text = "MỚI", Selected = customer.Rank == "MỚI" },
                new SelectListItem { Value = "ĐỒNG", Text = "ĐỒNG", Selected = customer.Rank == "ĐỒNG" },
                new SelectListItem { Value = "BẠC", Text = "BẠC", Selected = customer.Rank == "BẠC" },
                new SelectListItem { Value = "VÀNG", Text = "VÀNG", Selected = customer.Rank == "VÀNG" },
                new SelectListItem { Value = "KIM CƯƠNG", Text = "KIM CƯƠNG", Selected = customer.Rank == "KIM CƯƠNG" }
            };

            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}