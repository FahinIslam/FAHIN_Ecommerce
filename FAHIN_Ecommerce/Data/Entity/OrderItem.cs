using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAHIN_Ecommerce.Data.Entity
{

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
