using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAHIN_Ecommerce.Data.Entity
{
    public class Product:BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;
        [Required]
        public string description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal price { get; set; }
        public int stockQuantity { get; set; }
        public string? imageUrl { get; set; }
        public int categoryId { get; set; }

        [ForeignKey("categoryId")]
        public virtual Category category { get; set; } = null!;
    }
}
