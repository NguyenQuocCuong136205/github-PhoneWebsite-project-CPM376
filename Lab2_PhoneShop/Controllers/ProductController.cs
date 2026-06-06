using Lab2_PhoneShop.DTOs;
using Lab2_PhoneShop.Models;
using Lab2_PhoneShop.PhoneShopDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Lab2_PhoneShop.Controllers
{
    public class ProductController : Controller
    {
        public const string CARTKEY = "cart";
        PhoneShopDBContext _ctx;
        public ProductController(PhoneShopDBContext ctx)
        {
            _ctx = ctx;
        }

        // Lấy cart từ Session (danh sách CartItem)
        List<Lab2_PhoneShop.DTOs.CartDTO>? GetCartItems()
        {

            var session = HttpContext.Session;
            string? jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<Lab2_PhoneShop.DTOs.CartDTO>>(jsoncart);
            }
            return new List<Lab2_PhoneShop.DTOs.CartDTO>();
        }

        // Xóa cart khỏi session
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        void SaveCartSession(List<Lab2_PhoneShop.DTOs.CartDTO> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

        public async Task<IActionResult> Index()
        {
            var products = await _ctx.Products.ToListAsync();
            return View(products);
        }

        [Route("/Product/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var product = await _ctx.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Slug != null && p.Slug.Trim().ToLower() == slug.Trim().ToLower());

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [Route("/ViewCart")]
        public IActionResult ViewCart()
        {
            return View();
        }

        [Route("/Cart/AddToCart")]
        public async Task<IActionResult> AddToCart(int pid, int quantity)
        {
            int q = quantity < 1 ? 1 : quantity;
            Product? prod = await _ctx.Products.FirstOrDefaultAsync(p => p.Id == pid);

            if (prod != null)
            {
                List<Lab2_PhoneShop.DTOs.CartDTO> ls = GetCartItems() ?? new List<Lab2_PhoneShop.DTOs.CartDTO>();
                var cartItem = ls.FirstOrDefault(x => x.Product?.Id == pid);
                if (cartItem != null)
                {
                    cartItem.Quantity += q;
                }
                else
                {
                    Lab2_PhoneShop.DTOs.CartDTO dto = new CartDTO
                    {
                        Product = prod,
                        Quantity = q
                    };
                    ls.Add(dto);
                }
                SaveCartSession(ls);
            }
            
            return RedirectToAction("Index", "Cart");
        }
    }
}
