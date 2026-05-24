namespace FAHIN_Ecommerce.Services
{
    public interface IGlobalDataService
    {
        string GetSiteName();
        string GetAdminEmail();
    }

    public class GlobalDataService : IGlobalDataService
    {
        public string GetSiteName() => "FAHIN Ecommerce";
        public string GetAdminEmail() => "admin@fahin.com";
    }
}
