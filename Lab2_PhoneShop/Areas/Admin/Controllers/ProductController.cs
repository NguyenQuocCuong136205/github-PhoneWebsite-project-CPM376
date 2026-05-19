using Lab2_PhoneShop.Models;
using Lab2_PhoneShop.PhoneShopDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab2_PhoneShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        PhoneShopDBContext db;

        public ProductController(PhoneShopDBContext db)
        {
            this.db = db;
        }

        public async Task<ActionResult> Index()
        {
            var products = await db.Products.ToListAsync();
            return View(products);
        }

        public async Task<ActionResult> Create()
        {
            var carts = await db.Categories.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToListAsync();
            ViewBag.CategoryId = carts;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            await db.Products.AddAsync(product);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
