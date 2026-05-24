using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAHIN_Ecommerce.Data.Entity
{
    public class Category : BaseEntity
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
        // Removed ICollection<Category> subCategories for one-way binding
        // Removed ICollection<Product> products for one-way binding
    }

    public class Product : BaseEntity
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

    public class Order : BaseEntity
    {
        [Required]
        public string userId { get; set; } = string.Empty;
        [ForeignKey("userId")]
        public virtual ApplicationUser user { get; set; } = null!;
        
        public DateTime orderDate { get; set; } = DateTime.UtcNow;
        [Column(TypeName = "decimal(18,2)")]
        public decimal totalAmount { get; set; }
        public string status { get; set; } = "Pending";
        
        public string? shippingAddress { get; set; }
        public string? paymentMethod { get; set; }
        
        // Removed ICollection<OrderItem> orderItems for one-way binding
    }

    public class OrderItem : BaseEntity
    {
        public int orderId { get; set; }
        [ForeignKey("orderId")]
        public virtual Order order { get; set; } = null!;
        
        public int productId { get; set; }
        [ForeignKey("productId")]
        public virtual Product product { get; set; } = null!;
        
        public int quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal unitPrice { get; set; }
    }
}
