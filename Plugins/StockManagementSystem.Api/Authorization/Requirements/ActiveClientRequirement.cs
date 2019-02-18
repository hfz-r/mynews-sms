using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Api.Authorization.Requirements
{
    public class ActiveClientRequirement : IAuthorizationRequirement
    {
        public bool IsClientActive()
        {
            if (!ClientExistsAndActive())
            {
                return false;
            }

            return true;
        }

        private bool ClientExistsAndActive()
        {
            var httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

            var clientId = httpContextAccessor.HttpContext.User.FindFirst("client_id")?.Value;
            if (clientId != null)
            {
                var clientService = EngineContext.Current.Resolve<IClientService>();
                var client = clientService.FindClientByClientIdAsync(clientId).GetAwaiter().GetResult();
                if (client != null && client.Enabled)
                {
                    return true;
                }
            }

            return false;
        }
    }
}