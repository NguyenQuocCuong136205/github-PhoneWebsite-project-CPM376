using Lab2_PhoneShop.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_PhoneShop.PhoneShopDB
{
    public class PhoneShopDBContext : DbContext
    {
        public PhoneShopDBContext(DbContextOptions<PhoneShopDBContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}
