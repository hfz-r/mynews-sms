using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using StockManagementSystem.Api.Authorization.Requirements;

namespace StockManagementSystem.Api.Authorization.Policies
{
    public class ActiveApiAuthorizationPolicy : AuthorizationHandler<ActiveApiRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveApiRequirement requirement)
        {
            if (await requirement.IsActive())
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}