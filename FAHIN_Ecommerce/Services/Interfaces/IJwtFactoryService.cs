using System.Security.Claims;
using System.Threading.Tasks;

namespace FAHIN_Ecommerce.Services.Interfaces
{
    public interface IJwtFactoryService
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id);
    }
}
