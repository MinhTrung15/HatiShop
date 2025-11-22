// Controllers/StaffsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HatiShop.Models;
using HatiShop.Services;
using static HatiShop.Services.StaffService;

namespace HatiShop.Controllers
{
    public class StaffsController : Controller
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        // GET: Staffs
        public async Task<IActionResult> Index(string searchType, string searchValue)
        {
            ViewBag.SearchTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "all", Text = "Tất cả" },
                new SelectListItem { Value = "name", Text = "Theo tên" },
                new SelectListItem { Value = "id", Text = "Theo mã" },
                new SelectListItem { Value = "phone", Text = "Theo SĐT" },
                new SelectListItem { Value = "role", Text = "Theo vai trò" }
            };

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Quản lý", Text = "Quản lý" },
                new SelectListItem { Value = "Nhân viên", Text = "Nhân viên" },
                new SelectListItem { Value = "Thu ngân", Text = "Thu ngân" },
                new SelectListItem { Value = "Kho", Text = "Nhân viên kho" }
            };

            IEnumerable<Staff> staffs;

            if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrEmpty(searchType) && searchType != "all")
            {
                staffs = await _staffService.SearchStaffAsync(searchType, searchValue);
                ViewBag.SearchType = searchType;
                ViewBag.SearchValue = searchValue;
            }
            else
            {
                staffs = await _staffService.GetAllStaffAsync();
            }

            return View(staffs);
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            ViewBag.Genders = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác" }
            };

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Quản lý", Text = "Quản lý" },
                new SelectListItem { Value = "Nhân viên", Text = "Nhân viên" },
                new SelectListItem { Value = "Thu ngân", Text = "Thu ngân" },
                new SelectListItem { Value = "Kho", Text = "Nhân viên kho" }
            };

            return View();
        }

        // POST: Staffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,FullName,Gender,BirthDate,PhoneNumber,Email,Address,Role,AvatarFile")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                var result = await _staffService.CreateStaffAsync(staff, staff.AvatarFile);
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

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Quản lý", Text = "Quản lý" },
                new SelectListItem { Value = "Nhân viên", Text = "Nhân viên" },
                new SelectListItem { Value = "Thu ngân", Text = "Thu ngân" },
                new SelectListItem { Value = "Kho", Text = "Nhân viên kho" }
            };

            return View(staff);
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            ViewBag.Genders = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam", Selected = staff.Gender == "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ", Selected = staff.Gender == "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác", Selected = staff.Gender == "Khác" }
            };

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Quản lý", Text = "Quản lý", Selected = staff.Role == "Quản lý" },
                new SelectListItem { Value = "Nhân viên", Text = "Nhân viên", Selected = staff.Role == "Nhân viên" },
                new SelectListItem { Value = "Thu ngân", Text = "Thu ngân", Selected = staff.Role == "Thu ngân" },
                new SelectListItem { Value = "Kho", Text = "Nhân viên kho", Selected = staff.Role == "Kho" }
            };

            return View(staff);
        }

        // POST: Staffs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Username,Password,FullName,Gender,BirthDate,PhoneNumber,Email,Address,Role,AvatarFile")] Staff staff)
        {
            if (id != staff.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _staffService.UpdateStaffAsync(staff, staff.AvatarFile);
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
                new SelectListItem { Value = "Nam", Text = "Nam", Selected = staff.Gender == "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ", Selected = staff.Gender == "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác", Selected = staff.Gender == "Khác" }
            };

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Quản lý", Text = "Quản lý", Selected = staff.Role == "Quản lý" },
                new SelectListItem { Value = "Nhân viên", Text = "Nhân viên", Selected = staff.Role == "Nhân viên" },
                new SelectListItem { Value = "Thu ngân", Text = "Thu ngân", Selected = staff.Role == "Thu ngân" },
                new SelectListItem { Value = "Kho", Text = "Nhân viên kho", Selected = staff.Role == "Kho" }
            };

            return View(staff);
        }

        // GET: Staffs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _staffService.DeleteStaffAsync(id);
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