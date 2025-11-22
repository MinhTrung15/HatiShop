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

            IEnumerable<Customer> customers;

            if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrEmpty(searchType) && searchType != "all")
            {
                customers = await _customerService.SearchCustomersAsync(searchType, searchValue);
                ViewBag.SearchType = searchType;
                ViewBag.SearchValue = searchValue;
            }
            else
            {
                customers = await _customerService.GetAllCustomersAsync();
            }

            return View(customers);
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
            ViewBag.Genders = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác" }
            };

            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,FullName,Gender,BirthDate,PhoneNumber,Email,Address,AvatarFile")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.CreateCustomerAsync(customer, customer.AvatarFile);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            ViewBag.Genders = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác" }
            };

            return View(customer);
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

            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Username,Password,FullName,Gender,BirthDate,PhoneNumber,Email,Address,Revenue,Rank,AvatarFile")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _customerService.UpdateCustomerAsync(customer, customer.AvatarFile);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            ViewBag.Genders = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam", Selected = customer.Gender == "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ", Selected = customer.Gender == "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác", Selected = customer.Gender == "Khác" }
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