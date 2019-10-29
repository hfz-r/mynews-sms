using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using StockManagementSystem.Api.Authorization.Requirements;

namespace StockManagementSystem.Api.Authorization.Policies
{
    public class ValidSchemeAuthorizationPolicy : AuthorizationHandler<AuthorizationSchemeRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationSchemeRequirement requirement)
        {
            var mvcContext = context.Resource as AuthorizationFilterContext;

            if (await requirement.IsValid(mvcContext?.HttpContext.Request.Headers))
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