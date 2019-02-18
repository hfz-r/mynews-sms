using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using StockManagementSystem.Api.Authorization.Requirements;

namespace StockManagementSystem.Api.Authorization.Policies
{
    public class ActiveApiAuthorizationPolicy : AuthorizationHandler<ActiveApiRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveApiRequirement requirement)
        {
            if (requirement.IsActive())
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}