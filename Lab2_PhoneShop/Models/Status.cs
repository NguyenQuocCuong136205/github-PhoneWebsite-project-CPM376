using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2_PhoneShop.Models
{
    [Table("Status")]
    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Name { get; set; }
    }
}
