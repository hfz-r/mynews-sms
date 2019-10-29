using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Api.Authorization.Requirements
{
    public class ActiveLicenseRequirement : IAuthorizationRequirement
    {
        public async Task<bool> IsValid(IHeaderDictionary requestHeaders)
        {
            if (requestHeaders != null && requestHeaders.TryGetValue("Device-SerialNo", out var val))
            {
                var stateContext = SingletonList<string>.Instance.AsQueryable();
                var isMatch = await stateContext.FirstOrDefaultAsync(lic => lic == val.FirstOrDefault()) != null;

                return isMatch;
            }

            return false;
        }
    }
}