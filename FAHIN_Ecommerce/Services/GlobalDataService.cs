using FAHIN_Ecommerce.Services.Interfaces;

namespace FAHIN_Ecommerce.Services
{
    public class GlobalDataService : IGlobalDataService
    {
        public string GetSiteName() => "FAHIN Ecommerce";
        public string GetAdminEmail() => "admin@fahin.com";
    }
}
