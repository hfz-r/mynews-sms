using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Api.Authorization.Requirements
{
    public class ActiveClientRequirement : IAuthorizationRequirement
    {
        public async Task<bool> IsClientActive()
        {
            return await ClientExistsAndActive();
        }

        private static async Task<bool> ClientExistsAndActive()
        {
            var httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

            var clientId = httpContextAccessor.HttpContext.User.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (clientId != null)
            {
                var clientService = EngineContext.Current.Resolve<IClientService>();
                var client = await clientService.FindClientByClientIdAsync(clientId);
                if (client != null && client.Enabled)
                {
                    return true;
                }
            }

            return false;
        }
    }
}