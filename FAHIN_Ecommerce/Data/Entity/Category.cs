using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAHIN_Ecommerce.Data.Entity
{
    public class Category:BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? description { get; set; }
        public string? imageUrl { get; set; }
        public int? parentCategoryId { get; set; }

        [ForeignKey("parentCategoryId")]
        public virtual Category? parentCategory { get; set; }
        public virtual ICollection<Category> subCategories { get; set; } = new List<Category>();
        public virtual ICollection<Product> products { get; set; } = new List<Product>();
    }
}
