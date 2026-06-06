using Microsoft.AspNetCore.Mvc;

namespace Lab2_PhoneShop.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
