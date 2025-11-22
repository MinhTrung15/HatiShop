using Microsoft.AspNetCore.Mvc;
using HatiShop.Services;

namespace HatiShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomerService _customerService;

        public HomeController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                ViewBag.CustomerCount = customers?.Count() ?? 0;
            }
            catch
            {
                ViewBag.CustomerCount = 0;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}