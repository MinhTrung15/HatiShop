// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;

namespace HatiShop.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Customers");
        }
    }
}