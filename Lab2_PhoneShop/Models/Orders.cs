using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace Lab2_PhoneShop.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Column(TypeName = "Date")]
        public DateTime Date_time { get; set; }

        [ForeignKey("Promotion")]
        public int PromotionID { get; set; }
        public Promotion? Promotion { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string? ShippingPhone { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string? ShippingAddress { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }
    }
}
