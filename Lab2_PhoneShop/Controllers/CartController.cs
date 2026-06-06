using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lab2_PhoneShop.Controllers
{
    public class CartController : Controller
    {
        public const string CARTKEY = "cart";

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

        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            return View(cartItems);
        }

        public IActionResult Remove(int pid)
        {
            var ls = GetCartItems() ?? new List<Lab2_PhoneShop.DTOs.CartDTO>();
            var item = ls.FirstOrDefault(x => x.Product?.Id == pid);
            if (item != null)
            {
                ls.Remove(item);
                SaveCartSession(ls);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult IncreaseQuantityProduct(int productId)
        {
            var ls = GetCartItems() ?? new List<Lab2_PhoneShop.DTOs.CartDTO>();

            for (var i = 0; i < ls.Count; i++)
            {
                Console.WriteLine($"Product ID: {ls[i].Product?.Id}, Quantity: {ls[i].Quantity}");
                Console.WriteLine(productId);
            }

            if (ls != null)
            {
                var item = ls.FirstOrDefault(x => x.Product?.Id == productId);
                if (item != null)
                {
                    item.Quantity++;
                    SaveCartSession(ls);
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult DecreaseQuantityProduct(int productId)
        {
            var ls = GetCartItems() ?? new List<Lab2_PhoneShop.DTOs.CartDTO>();

            if (ls != null)
            {
                var item = ls.FirstOrDefault(x => x.Product?.Id == productId);
                if (item != null)
                {
                    if (item.Quantity > 1)
                    {
                        item.Quantity--;
                    }
                    else
                    {
                        ls.Remove(item);
                    }
                    SaveCartSession(ls);
                }
            }

            return RedirectToAction("Index");
        }
    }
}
