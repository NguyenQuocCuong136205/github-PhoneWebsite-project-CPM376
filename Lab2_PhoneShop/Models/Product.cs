using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2_PhoneShop.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string? Name { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string? Slug { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string? Photo { get; set; }
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public bool? Featured { get; set; }
    }
}
