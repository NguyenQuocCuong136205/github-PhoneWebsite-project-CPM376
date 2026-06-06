using Lab2_PhoneShop.Models;

namespace Lab2_PhoneShop.DTOs
{
    public class CartDTO
    {
        public Product? Product { get; set; }
        public int Quantity { get; set; }
    }
}
