using Lab2_PhoneShop.Models;
using Lab2_PhoneShop.PhoneShopDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Lab2_PhoneShop.Controllers
{
    public class HomeController : Controller
    {
        PhoneShopDBContext _ctx;
        public HomeController(PhoneShopDBContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var FeaturedProducts = await  _ctx.Products.Where(p => p.Featured == true).Take(10).ToListAsync();
            ViewBag.FeaturedProducts = FeaturedProducts;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Products() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
