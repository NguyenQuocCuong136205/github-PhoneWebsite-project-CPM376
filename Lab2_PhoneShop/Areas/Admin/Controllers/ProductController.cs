using Lab2_PhoneShop.Models;
using Lab2_PhoneShop.PhoneShopDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2_PhoneShop.DTOs.Products;

namespace Lab2_PhoneShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        PhoneShopDBContext db;
        IWebHostEnvironment env;

        public ProductController(PhoneShopDBContext db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
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
        public async Task<IActionResult> Create(CreateProductDTO p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            string? imgFileName = string.Empty;
            if (p.Photo != null && p.Photo.Length > 0)
            {
                try
                {
                    var imgFolder = Path.Combine(env.WebRootPath, "img");
                    if (!Directory.Exists(imgFolder))
                    {
                        Directory.CreateDirectory(imgFolder);
                    }

                    string? imgPath = Path.Combine(imgFolder, p.Photo.FileName);

                    using (var fs = new FileStream(imgPath, FileMode.Create))
                    {
                        await p.Photo.CopyToAsync(fs);
                    }
                    imgFileName = p.Photo.FileName;
                }

                catch (Exception ex)
                {
                    ViewBag.Message = $"Error uploading image:" + ex.Message;
                    return View(p);
                }
            }

            Category? cate = null;
            if (p.CategoryId.HasValue)
            {
                cate = await db.Categories.FindAsync(p.CategoryId.Value);
            }

            var product = new Product
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                //sal = p.PriceSale,
                Category = cate,
                Photo = imgFileName
            };

            await db.Products.AddAsync(product);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
