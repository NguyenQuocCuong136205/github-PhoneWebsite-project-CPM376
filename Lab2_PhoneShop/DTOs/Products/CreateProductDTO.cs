namespace Lab2_PhoneShop.DTOs.Products
{
    public class CreateProductDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceSale { get; set; }
        public IFormFile? Photo { get; set; }
        public int? CategoryId { get; set; }
    }
}
