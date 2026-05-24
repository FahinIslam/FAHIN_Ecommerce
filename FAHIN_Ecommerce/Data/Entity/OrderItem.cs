using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAHIN_Ecommerce.Data.Entity
{

    public class OrderItem : BaseEntity
    {
        public int? orderId { get; set; }
        [ForeignKey("orderId")]
        public  Order order { get; set; } 
        
        public int? productId { get; set; }
        [ForeignKey("productId")]
        public  Product product { get; set; } 
        
        public int? quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? unitPrice { get; set; }
    }
}
