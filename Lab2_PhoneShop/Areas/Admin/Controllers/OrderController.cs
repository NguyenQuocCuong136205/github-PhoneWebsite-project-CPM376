using Lab2_PhoneShop.Models;
using Lab2_PhoneShop.PhoneShopDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab2_PhoneShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly PhoneShopDBContext _ctx;

        public OrderController(PhoneShopDBContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            // 1. General stats
            int totalOrders = await _ctx.orders.CountAsync();
            decimal totalRevenue = await _ctx.orderDetails.SumAsync(od => (decimal?)(od.Price * od.Quantity)) ?? 0;
            int totalProductsSold = await _ctx.orderDetails.SumAsync(od => (int?)od.Quantity) ?? 0;
            decimal avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // 2. Recent orders
            var recentOrders = await _ctx.orders
                .Include(o => o.User)
                .Include(o => o.Status)
                .OrderByDescending(o => o.Date_time)
                .Take(10)
                .Select(o => new RecentOrderDTO
                {
                    OrderId = o.Id,
                    CustomerName = o.User != null ? o.User.Name : "Default Customer",
                    CustomerPhone = o.ShippingPhone ?? "",
                    Address = o.ShippingAddress ?? "",
                    Date = o.Date_time,
                    Status = o.Status != null ? o.Status.Name : "Pending",
                    TotalAmount = _ctx.orderDetails.Where(od => od.OrderId == o.Id).Sum(od => od.Price * od.Quantity)
                })
                .ToListAsync();

            // 3. Top selling products
            var topProducts = await _ctx.orderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new TopProductDTO
                {
                    ProductId = g.Key,
                    ProductName = _ctx.Products.Where(p => p.Id == g.Key).Select(p => p.Name).FirstOrDefault() ?? "Unknown Product",
                    ProductPhoto = _ctx.Products.Where(p => p.Id == g.Key).Select(p => p.Photo).FirstOrDefault() ?? "",
                    Price = _ctx.Products.Where(p => p.Id == g.Key).Select(p => p.Price).FirstOrDefault() ?? 0,
                    QuantitySold = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Price * od.Quantity)
                })
                .OrderByDescending(tp => tp.QuantitySold)
                .Take(5)
                .ToListAsync();

            // 4. Daily revenue for the last 14 days
            var dailyRevenues = new List<DailyRevenueDTO>();
            var today = DateTime.Today;
            var startDate = today.AddDays(-13);

            for (int i = 0; i < 14; i++)
            {
                var date = startDate.AddDays(i);
                var nextDate = date.AddDays(1);

                var dailyRevenue = await _ctx.orderDetails
                    .Include(od => od.Order)
                    .Where(od => od.Order!.Date_time >= date && od.Order!.Date_time < nextDate)
                    .SumAsync(od => (decimal?)(od.Price * od.Quantity)) ?? 0;

                dailyRevenues.Add(new DailyRevenueDTO
                {
                    DateLabel = date.ToString("dd/MM"),
                    Revenue = dailyRevenue
                });
            }

            var viewModel = new DashboardViewModel
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalProductsSold = totalProductsSold,
                AverageOrderValue = avgOrderValue,
                RecentOrders = recentOrders,
                TopProducts = topProducts,
                DailyRevenues = dailyRevenues
            };

            return View(viewModel);
        }
    }

    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProductsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<RecentOrderDTO> RecentOrders { get; set; } = new List<RecentOrderDTO>();
        public List<TopProductDTO> TopProducts { get; set; } = new List<TopProductDTO>();
        public List<DailyRevenueDTO> DailyRevenues { get; set; } = new List<DailyRevenueDTO>();
    }

    public class RecentOrderDTO
    {
        public int OrderId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? Address { get; set; }
        public DateTime Date { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class TopProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductPhoto { get; set; } = "";
        public decimal Price { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class DailyRevenueDTO
    {
        public string DateLabel { get; set; } = "";
        public decimal Revenue { get; set; }
    }
}
