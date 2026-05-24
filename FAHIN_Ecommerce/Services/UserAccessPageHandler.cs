using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace FAHIN_Ecommerce.Services
{
    public class UserAccessPageHandler : AuthorizationHandler<UserAccessPageRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAccessPageRequirement requirement)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
