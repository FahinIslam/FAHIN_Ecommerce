using Microsoft.IdentityModel.Tokens;

namespace FAHIN_Ecommerce.Models
{
    public class JwtIssuerOptions
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? Subject { get; set; }
        public string? SecreatKey { get; set; }
        public SigningCredentials? SigningCredentials { get; set; }
    }
}
