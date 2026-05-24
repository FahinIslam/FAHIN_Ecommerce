using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FAHIN_Ecommerce.Data.Entity
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string fullName { get; set; } = string.Empty;
        public string? profilePicture { get; set; }
        public DateTime? createdAt { get; set; } = DateTime.UtcNow;
        public bool isActive { get; set; } = true;
    }

    public class ApplicationRole : IdentityRole
    {
        public string? description { get; set; }
    }
}
