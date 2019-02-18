using Microsoft.AspNetCore.Authorization;
using StockManagementSystem.Api.Domain;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Api.Authorization.Requirements
{
    public class ActiveApiRequirement : IAuthorizationRequirement
    {
        public bool IsActive()
        {
            var setting = EngineContext.Current.Resolve<ApiSettings>();

            if (setting.EnableApi)
            {
                return true;
            }

            return false;
        }
    }
}