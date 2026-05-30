using Microsoft.AspNetCore.Mvc;

namespace Lab2_PhoneShop.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }

    }
}
