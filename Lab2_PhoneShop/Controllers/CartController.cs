using Microsoft.AspNetCore.Mvc;

namespace Lab2_PhoneShop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }
    }
}
