using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Configuration;

namespace StockManagementSystem.Services.Configuration
{
    public interface ITenantService
    {
        bool ContainsHostValue(Tenant tenant, string host);
        Task<Tenant> GetTenantByIdAsync(int tenantId, bool loadCacheableCopy = true);
        Task<IList<Tenant>> GetTenantsAsync(bool loadCacheableCopy = true);
        string[] ParseHostValues(Tenant tenant);
    }
}