using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using StockManagementSystem.Api.Authorization.Requirements;

namespace StockManagementSystem.Api.Authorization.Policies
{
    public class ActiveClientAuthorizationPolicy : AuthorizationHandler<ActiveClientRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveClientRequirement requirement)
        {
            if (await requirement.IsClientActive())
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