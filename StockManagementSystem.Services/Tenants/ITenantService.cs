using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Tenants;

namespace StockManagementSystem.Services.Tenants
{
    public interface ITenantService
    {
        bool ContainsHostValue(Tenant tenant, string host);

        Task<Tenant> GetTenantByIdAsync(int tenantId, bool loadCacheableCopy = true);

        Task<IList<Tenant>> GetTenantsAsync(bool loadCacheableCopy = true);

        string[] ParseHostValues(Tenant tenant);

        #region Synchronous wrapper

        IList<Tenant> GetTenants(bool loadCacheableCopy = true);

        Tenant GetTenantById(int tenantId, bool loadCacheableCopy = true);

        #endregion
    }
}