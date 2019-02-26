using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Tenants;

namespace StockManagementSystem.Services.Tenants
{
    public interface ITenantMappingService
    {
        int[] GetTenantsIdsWithAccess<T>(T entity) where T : BaseEntity, ITenantMappingSupported;

        bool Authorize<T>(T entity) where T : BaseEntity, ITenantMappingSupported;

        bool Authorize<T>(T entity, int tenantId) where T : BaseEntity, ITenantMappingSupported;

        Task DeleteTenantMapping(TenantMapping tenantMapping);

        Task<TenantMapping> GetTenantMappingById(int tenantMappingId);

        Task<IList<TenantMapping>> GetTenantMappings<T>(T entity) where T : BaseEntity, ITenantMappingSupported;

        Task InsertTenantMapping<T>(T entity, int tenantId) where T : BaseEntity, ITenantMappingSupported;
    }
}