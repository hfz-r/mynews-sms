using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Tenants;
using StockManagementSystem.Data.Extensions;

namespace StockManagementSystem.Services.Tenants
{
    /// <summary>
    /// Tenant mapping service
    /// </summary>
    public class TenantMappingService : ITenantMappingService
    {
        private readonly RecordSettings _recordSettings;
        private readonly IRepository<TenantMapping> _tenantMappingRepository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ITenantContext _tenantContext;

        public TenantMappingService(
            RecordSettings recordSettings, 
            IRepository<TenantMapping> tenantMappingRepository, 
            IStaticCacheManager cacheManager, 
            ITenantContext tenantContext)
        {
            _recordSettings = recordSettings;
            _tenantMappingRepository = tenantMappingRepository;
            _cacheManager = cacheManager;
            _tenantContext = tenantContext;
        }

        public async Task DeleteTenantMapping(TenantMapping tenantMapping)
        {
            if (tenantMapping == null)
                throw new ArgumentNullException(nameof(tenantMapping));

            await _tenantMappingRepository.DeleteAsync(tenantMapping);

            _cacheManager.RemoveByPattern(TenantDefaults.TenantMappingPatternCacheKey);
        }

        public async Task<TenantMapping> GetTenantMappingById(int tenantMappingId)
        {
            if (tenantMappingId == 0)
                return null;

            return await _tenantMappingRepository.GetByIdAsync(tenantMappingId);
        }

        public async Task<IList<TenantMapping>> GetTenantMappings<T>(T entity) where T : BaseEntity, ITenantMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var query = from sm in _tenantMappingRepository.Table
                where sm.EntityId == entityId && sm.EntityName == entityName
                select sm;

            var tenantMappings = await query.ToListAsync();

            return tenantMappings;
        }

        protected async Task InsertTenantMapping(TenantMapping tenantMapping)
        {
            if (tenantMapping == null)
                throw new ArgumentNullException(nameof(tenantMapping));

            await _tenantMappingRepository.InsertAsync(tenantMapping);

            _cacheManager.RemoveByPattern(TenantDefaults.TenantMappingPatternCacheKey);
        }

        public async Task InsertTenantMapping<T>(T entity, int tenantId) where T : BaseEntity, ITenantMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (tenantId == 0)
                throw new ArgumentOutOfRangeException(nameof(tenantId));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var tenantMapping = new TenantMapping
            {
                EntityId = entityId,
                EntityName = entityName,
                TenantId = tenantId
            };

            await InsertTenantMapping(tenantMapping);
        }

        /// <summary>
        /// Find tenant identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Tenant identifiers</returns>
        public virtual int[] GetTenantsIdsWithAccess<T>(T entity) where T : BaseEntity, ITenantMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var key = string.Format(TenantDefaults.TenantMappingByEntityIdNameCacheKey, entityId, entityName);
            return _cacheManager.Get(key, () =>
            {
                var query = from sm in _tenantMappingRepository.Table
                    where sm.EntityId == entityId &&
                          sm.EntityName == entityName
                    select sm.TenantId;
                return query.ToArray();
            });
        }

        /// <summary>
        /// Authorize whether entity could be accessed in the current tenant (mapped to this tenant)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity) where T : BaseEntity, ITenantMappingSupported
        {
            return Authorize(entity, _tenantContext.CurrentTenant.Id);
        }

        /// <summary>
        /// Authorize whether entity could be accessed in a tenant (mapped to this tenant)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="tenantId">Tenant identifier</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity, int tenantId) where T : BaseEntity, ITenantMappingSupported
        {
            if (entity == null)
                return false;

            if (tenantId == 0)
                //return true if no tenant specified/found
                return true;

            if (_recordSettings.IgnoreTenantLimitations)
                return true;

            if (!entity.LimitedToTenants)
                return true;

            foreach (var tenantIdWithAccess in GetTenantsIdsWithAccess(entity))
                if (tenantId == tenantIdWithAccess)
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }
    }
}