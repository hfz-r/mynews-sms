using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using StockManagementSystem.Api.Domain;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Api.Authorization.Requirements
{
    public class ActiveApiRequirement : IAuthorizationRequirement
    {
        public async Task<bool> IsActive()
        {
            return await Task.FromResult(EngineContext.Current.Resolve<ApiSettings>().EnableApi);
        }
    }
}