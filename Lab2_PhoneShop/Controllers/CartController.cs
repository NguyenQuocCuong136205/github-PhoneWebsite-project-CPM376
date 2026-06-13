using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Lab2_PhoneShop.Models;
using Lab2_PhoneShop.PhoneShopDB;
using Microsoft.EntityFrameworkCore;
using Lab2_PhoneShop.DTOs;

namespace Lab2_PhoneShop.Controllers
{
    public class CartController : Controller
    {
        public const string CARTKEY = "cart";
        private readonly PhoneShopDBContext _ctx;
        private readonly BookCart.Service.IPayPalService _payPalService;

        public CartController(PhoneShopDBContext ctx, BookCart.Service.IPayPalService payPalService)
        {
            _ctx = ctx;
            _payPalService = payPalService;
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

        private Status GetOrCreateDefaultStatus()
        {
            var status = _ctx.statuses.FirstOrDefault(s => s.Name == "Pending");
            if (status == null)
            {
                status = new Status { Name = "Pending" };
                _ctx.statuses.Add(status);
                _ctx.SaveChanges();
            }
            return status;
        }

        private Promotion GetOrCreateDefaultPromotion()
        {
            var promotion = _ctx.promotions.FirstOrDefault(p => p.Code == "NONE");
            if (promotion == null)
            {
                promotion = new Promotion
                {
                    Code = "NONE",
                    Name = "No Promotion",
                    Type = "None",
                    Discount = 0,
                    Description = "Default Promotion",
                    StartDate = DateTime.Now.AddYears(-1),
                    EndDate = DateTime.Now.AddYears(10)
                };
                _ctx.promotions.Add(promotion);
                _ctx.SaveChanges();
            }
            return promotion;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(string name, string phone, string address, string opt)
        {
            var cartItems = GetCartItems();
            if (cartItems == null || !cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống. Không thể thanh toán.";
                return RedirectToAction("Index");
            }

            decimal total = cartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity);

            if (opt == "PayPal")
            {
                // Save billing details in session
                HttpContext.Session.SetString("Checkout_Name", name);
                HttpContext.Session.SetString("Checkout_Phone", phone);
                HttpContext.Session.SetString("Checkout_Address", address);

                var paymentInfo = new PaymentInformation
                {
                    fullNname = name,
                    phone = phone,
                    address = address,
                    Amount = total,
                    Description = $"Thanh toan don hang PayPal tu {name}"
                };

                try
                {
                    var approvalUrl = await _payPalService.CreatePaymentUrl(paymentInfo, HttpContext);
                    if (!string.IsNullOrEmpty(approvalUrl))
                    {
                        return Redirect(approvalUrl);
                    }
                    else
                    {
                        TempData["Error"] = "Không thể tạo liên kết thanh toán PayPal.";
                        return RedirectToAction("Failure");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi kết nối PayPal: " + ex.Message;
                    return RedirectToAction("Failure");
                }
            }
            else
            {
                // COD / Direct Bank transfer
                try
                {
                    var user = GetOrCreateDefaultUser();
                    var status = GetOrCreateDefaultStatus();
                    var promotion = GetOrCreateDefaultPromotion();

                    var order = new Order
                    {
                        UserId = user.Id,
                        Date_time = DateTime.Now,
                        PromotionID = promotion.Id,
                        ShippingPhone = phone,
                        ShippingAddress = address,
                        StatusId = status.Id
                    };
                    _ctx.orders.Add(order);
                    _ctx.SaveChanges();

                    foreach (var item in cartItems)
                    {
                        if (item.Product != null)
                        {
                            var detail = new OrderDetails
                            {
                                OrderId = order.Id,
                                ProductId = item.Product.Id,
                                Price = item.Product.Price ?? 0,
                                PriceSale = 0,
                                Quantity = item.Quantity
                            };
                            _ctx.orderDetails.Add(detail);
                        }
                    }
                    _ctx.SaveChanges();

                    // Clear Cart
                    ClearCart();

                    return RedirectToAction("Success", new { orderId = order.Id });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi lưu đơn hàng: " + ex.Message;
                    return RedirectToAction("Failure");
                }
            }
        }

        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                var response = await _payPalService.PaymentExecute(Request.Query);
                if (response.Success)
                {
                    var name = HttpContext.Session.GetString("Checkout_Name") ?? "Customer";
                    var phone = HttpContext.Session.GetString("Checkout_Phone") ?? "";
                    var address = HttpContext.Session.GetString("Checkout_Address") ?? "";

                    var user = GetOrCreateDefaultUser();
                    var status = GetOrCreateDefaultStatus();
                    var promotion = GetOrCreateDefaultPromotion();

                    var order = new Order
                    {
                        UserId = user.Id,
                        Date_time = DateTime.Now,
                        PromotionID = promotion.Id,
                        ShippingPhone = phone,
                        ShippingAddress = address,
                        StatusId = status.Id
                    };
                    _ctx.orders.Add(order);
                    _ctx.SaveChanges();

                    var cartItems = GetCartItems();
                    foreach (var item in cartItems)
                    {
                        if (item.Product != null)
                        {
                            var detail = new OrderDetails
                            {
                                OrderId = order.Id,
                                ProductId = item.Product.Id,
                                Price = item.Product.Price ?? 0,
                                PriceSale = 0,
                                Quantity = item.Quantity
                            };
                            _ctx.orderDetails.Add(detail);
                        }
                    }
                    _ctx.SaveChanges();

                    // Clear Cart
                    ClearCart();

                    return RedirectToAction("Success", new { orderId = order.Id });
                }
                else
                {
                    TempData["Error"] = response.ErrorMessage ?? "Thanh toán PayPal không thành công.";
                    return RedirectToAction("Failure");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xử lý phản hồi từ PayPal: " + ex.Message;
                return RedirectToAction("Failure");
            }
        }

        public IActionResult Success(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        public IActionResult Failure()
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

        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var ls = GetCartItems() ?? new List<Lab2_PhoneShop.DTOs.CartDTO>();

            if (ls != null)
            {
                var item = ls.FirstOrDefault(x => x.Product?.Id == productId);
                if (item != null)
                {
                    if (quantity > 0)
                    {
                        item.Quantity = quantity;
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
