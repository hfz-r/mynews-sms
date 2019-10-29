using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Tenants;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Tenants;

namespace StockManagementSystem.Web
{
    public class TenantContext : ITenantContext
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantService _tenantService;

        private Tenant _cachedTenant;
        private int? _cachedActiveTenantScopeConfiguration;

        public TenantContext(
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ITenantService tenantService)
        {
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _tenantService = tenantService;
        }

        public virtual Tenant CurrentTenant
        {
            get
            {
                if (_cachedTenant != null)
                    return _cachedTenant;

                //try to determine the current tenant by HOST header
                string host = _httpContextAccessor.HttpContext?.Request?.Headers[HeaderNames.Host];

                var allTenants = _tenantService.GetTenants();
                var tenant = allTenants.FirstOrDefault(s => _tenantService.ContainsHostValue(s, host)) ?? allTenants.FirstOrDefault();

                _cachedTenant = tenant ?? throw new Exception("No tenant could be loaded");

                return _cachedTenant;
            }
        }

        public virtual int ActiveTenantScopeConfiguration
        {
            get
            {
                if (_cachedActiveTenantScopeConfiguration.HasValue)
                    return _cachedActiveTenantScopeConfiguration.Value;

                //ensure that we have 2 (or more) tenants
                if (_tenantService.GetTenants().Count > 1)
                {
                    var currentUser = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;

                    //try to get tenant identifier from attributes
                    var tenantId = _genericAttributeService.GetAttribute<int>(currentUser,
                        UserDefaults.TenantScopeConfigurationAttribute);

                    _cachedActiveTenantScopeConfiguration = _tenantService.GetTenantById(tenantId)?.Id ?? 0;
                }
                else
                    _cachedActiveTenantScopeConfiguration = 0;

                return _cachedActiveTenantScopeConfiguration ?? 0;
            }
        }
    }
}