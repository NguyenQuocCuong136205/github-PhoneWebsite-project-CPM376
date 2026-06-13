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

        private User GetOrCreateDefaultUser()
        {
            var user = _ctx.users.FirstOrDefault();
            if (user == null)
            {
                var role = _ctx.roles.FirstOrDefault();
                if (role == null)
                {
                    role = new Role { Name = "Customer", Description = "Customer Role" };
                    _ctx.roles.Add(role);
                    _ctx.SaveChanges();
                }
                user = new User
                {
                    Name = "Default Customer",
                    Email = "customer@example.com",
                    Role_Id = role.Id,
                    Password = "123",
                    Phone = "0123456789",
                    Address = "Default Address"
                };
                _ctx.users.Add(user);
                _ctx.SaveChanges();
            }
            return user;
        }

        private Cart GetOrCreateCart(int userId)
        {
            var cart = _ctx.carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _ctx.carts.Add(cart);
                _ctx.SaveChanges();
            }
            return cart;
        }

        // Lấy cart từ Database & Session (danh sách CartItem)
        List<Lab2_PhoneShop.DTOs.CartDTO> GetCartItems()
        {
            var user = GetOrCreateDefaultUser();
            var cart = GetOrCreateCart(user.Id);
            
            var items = _ctx.cartItems
                .Where(ci => ci.CartId == cart.Id)
                .ToList();
                
            var result = new List<Lab2_PhoneShop.DTOs.CartDTO>();
            foreach (var item in items)
            {
                var product = _ctx.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    result.Add(new Lab2_PhoneShop.DTOs.CartDTO
                    {
                        Product = product,
                        Quantity = item.Quantity
                    });
                }
            }

            // Sync with session count for header display
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(result);
            session.SetString(CARTKEY, jsoncart);

            return result;
        }

        // Xóa cart khỏi database & session
        void ClearCart()
        {
            var user = GetOrCreateDefaultUser();
            var cart = GetOrCreateCart(user.Id);
            var dbItems = _ctx.cartItems.Where(ci => ci.CartId == cart.Id).ToList();
            _ctx.cartItems.RemoveRange(dbItems);
            _ctx.SaveChanges();

            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào database & session
        void SaveCartSession(List<Lab2_PhoneShop.DTOs.CartDTO> ls)
        {
            var user = GetOrCreateDefaultUser();
            var cart = GetOrCreateCart(user.Id);

            var dbItems = _ctx.cartItems.Where(ci => ci.CartId == cart.Id).ToList();

            foreach (var dto in ls)
            {
                if (dto.Product == null) continue;

                var dbItem = dbItems.FirstOrDefault(ci => ci.ProductId == dto.Product.Id);
                if (dbItem != null)
                {
                    dbItem.Quantity = dto.Quantity;
                    _ctx.cartItems.Update(dbItem);
                }
                else
                {
                    var newItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = dto.Product.Id,
                        Quantity = dto.Quantity
                    };
                    _ctx.cartItems.Add(newItem);
                }
            }

            foreach (var dbItem in dbItems)
            {
                if (!ls.Any(dto => dto.Product?.Id == dbItem.ProductId))
                {
                    _ctx.cartItems.Remove(dbItem);
                }
            }

            _ctx.SaveChanges();

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
