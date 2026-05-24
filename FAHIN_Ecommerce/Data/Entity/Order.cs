using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAHIN_Ecommerce.Data.Entity
{
    public class Order:BaseEntity
    {
        [Required]
        public string userId { get; set; } = string.Empty;
        [ForeignKey("userId")]
        public  ApplicationUser user { get; set; }

        public DateTime orderDate { get; set; } = DateTime.UtcNow.AddHours(6);
        [Column(TypeName = "decimal(18,2)")]
        public decimal? totalAmount { get; set; }
        public string status { get; set; } = "Pending";

        public string? shippingAddress { get; set; }
        public string? paymentMethod { get; set; }
    }
}
