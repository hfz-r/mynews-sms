using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Configuration;

namespace StockManagementSystem.Services.Configuration
{
    public class TenantService : ITenantService
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IStaticCacheManager _cacheManager;

        public TenantService(IRepository<Tenant> tenantRepository, IStaticCacheManager cacheManager)
        {
            _tenantRepository = tenantRepository;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Gets all tenants
        /// </summary>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached</param>
        /// <returns>Tenants</returns>
        public async Task<IList<Tenant>> GetTenantsAsync(bool loadCacheableCopy = true)
        {
            async Task<IList<Tenant>> LoadTenantsFunc()
            {
                var query = from s in _tenantRepository.Table orderby s.Id select s;
                return await query.ToListAsync();
            }

            if (loadCacheableCopy)
            {
                return _cacheManager.Get(TenantDefaults.TenantsAllCacheKey, () =>
                {
                    var result = new List<Tenant>();
                    foreach (var tenant in LoadTenantsFunc().GetAwaiter().GetResult())
                        result.Add(new TenantForCaching(tenant));
                    return result;
                });
            }

            return await LoadTenantsFunc();
        }

        /// <summary>
        /// Gets a tenant 
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached</param>
        /// <returns>Tenant</returns>
        public async Task<Tenant> GetTenantByIdAsync(int tenantId, bool loadCacheableCopy = true)
        {
            if (tenantId == 0)
                return null;

            async Task<Tenant> LoadTenantFunc()
            {
                return await _tenantRepository.GetByIdAsync(tenantId);
            }

            if (!loadCacheableCopy)
                return await LoadTenantFunc();

            var key = string.Format(TenantDefaults.TenantsByIdCacheKey, tenantId);
            return _cacheManager.Get(key, () =>
            {
                var tenant = LoadTenantFunc().GetAwaiter().GetResult();
                return tenant == null ? null : new TenantForCaching(tenant);
            });
        }

        /// <summary>
        /// Parse comma-separated Hosts
        /// </summary>
        /// <param name="tenant">Tenant</param>
        /// <returns>Comma-separated hosts</returns>
        public virtual string[] ParseHostValues(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            var parsedValues = new List<string>();
            if (string.IsNullOrEmpty(tenant.Hosts))
                return parsedValues.ToArray();

            var hosts = tenant.Hosts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var host in hosts)
            {
                var tmp = host.Trim();
                if (!string.IsNullOrEmpty(tmp))
                    parsedValues.Add(tmp);
            }

            return parsedValues.ToArray();
        }

        /// <summary>
        /// Indicates whether a tenant contains a specified host
        /// </summary>
        /// <param name="tenant">Tenant</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        public virtual bool ContainsHostValue(Tenant tenant, string host)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            if (string.IsNullOrEmpty(host))
                return false;

            var contains = this.ParseHostValues(tenant).Any(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase));

            return contains;
        }
    }
}