using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAHIN_Ecommerce.Data.Entity
{
    public class Order:BaseEntity
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

        public virtual ICollection<OrderItem> orderItems { get; set; } = new List<OrderItem>();
    }
}
