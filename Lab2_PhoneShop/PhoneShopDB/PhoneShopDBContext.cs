using Lab2_PhoneShop.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_PhoneShop.PhoneShopDB
{
    public class PhoneShopDBContext : DbContext
    {
        public PhoneShopDBContext(DbContextOptions<PhoneShopDBContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<OrderDetails> orderDetails { get; set; }

        public DbSet<Order> orders { get; set; }

        public DbSet<Promotion> promotions{ get; set; }

        public DbSet<Role> roles { get; set; }

        public DbSet<Status> statuses { get; set; }

        public DbSet<User> users { get; set; }

        public DbSet<Cart> carts { get; set; }

        public DbSet<CartItem> cartItems { get; set; }
    }
}
