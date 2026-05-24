using Microsoft.AspNetCore.Authorization;

namespace FAHIN_Ecommerce.Services
{
    public class UserAccessPageRequirement : IAuthorizationRequirement
    {
        public string? PageName { get; }
        public string? Permission { get; }

        public UserAccessPageRequirement(string? pageName, string? permission)
        {
            PageName = pageName;
            Permission = permission;
        }
    }
}
