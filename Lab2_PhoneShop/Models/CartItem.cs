using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2_PhoneShop.Models
{
    [Table("Cart_Items")]
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
