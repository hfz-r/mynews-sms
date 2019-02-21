using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Setting;
using StockManagementSystem.Models.Tenants;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Tenants;

namespace StockManagementSystem.Factories
{
    public class SettingModelFactory : ISettingModelFactory
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ITenantService _tenantService;
        private readonly IWorkContext _workContext;
        private readonly ITenantContext _tenantContext;

        public SettingModelFactory(
            IGenericAttributeService genericAttributeService, 
            ITenantService tenantService,
            IWorkContext workContext,
            ITenantContext tenantContext)
        {
            _genericAttributeService = genericAttributeService;
            _tenantService = tenantService;
            _workContext = workContext;
            _tenantContext = tenantContext;
        }

        public async Task<SettingModeModel> PrepareSettingModeModel(string modeName)
        {
            var model = new SettingModeModel
            {
                ModeName = modeName,
                Enabled = await _genericAttributeService.GetAttributeAsync<bool>(_workContext.CurrentUser, modeName)
            };

            return model;
        }

        /// <summary>
        /// Prepare tenant scope configuration model
        /// </summary>
        public async Task<TenantScopeConfigurationModel> PrepareTenantScopeConfigurationModel()
        {
            var model = new TenantScopeConfigurationModel
            {
                Tenants = (await _tenantService.GetTenantsAsync()).Select(t => t.ToModel<TenantModel>()).ToList(),
                TenantId = _tenantContext.ActiveTenantScopeConfiguration
            };

            return model;
        }
    }
}