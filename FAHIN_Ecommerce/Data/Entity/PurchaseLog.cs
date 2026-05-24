using System.ComponentModel.DataAnnotations.Schema;

namespace FAHIN_Ecommerce.Data.Entity
{
    public class PurchaseLog : BaseEntity
    {
        public string userId { get; set; } = string.Empty;
        public int productId { get; set; }
        public int quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal totalPrice { get; set; }
        public string? ipAddress { get; set; }
        public string? userAgent { get; set; }
        public string? details { get; set; }
    }
}
