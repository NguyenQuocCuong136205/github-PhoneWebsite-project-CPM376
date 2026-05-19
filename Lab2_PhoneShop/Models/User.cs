using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Lab2_PhoneShop.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Role")]
        public int Role_Id { get; set; }
        public Role? Role { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Email { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Password { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Name { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Phone { get; set; }

        [Column(TypeName = "text")]
        public string? Address { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Photo { get; set; }
    }
}
