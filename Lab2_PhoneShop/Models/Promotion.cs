using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2_PhoneShop.Models
{
    [Table("Promotion")]
    public class Promotion
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Type { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Description { get; set; }

        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime EndDate { get; set; }
    }
}
