using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Tenants;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Factories
{
    public class TenantMappingSupportedModelFactory : ITenantMappingSupportedModelFactory
    {
        private readonly ITenantMappingService _tenantMappingService;
        private readonly ITenantService _tenantService;

        public TenantMappingSupportedModelFactory(
            ITenantMappingService tenantMappingService, 
            ITenantService tenantService)
        {
            _tenantMappingService = tenantMappingService;
            _tenantService = tenantService;
        }

        /// <summary>
        /// Prepare selected and all available tenants for the passed model
        /// </summary>
        public async Task PrepareModelTenants<TModel>(TModel model) where TModel : ITenantMappingSupportedModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var availableTenants = await _tenantService.GetTenantsAsync();
            model.AvailableTenants = availableTenants.Select(tenant => new SelectListItem
            {
                Text = tenant.Name,
                Value = tenant.Id.ToString(),
                Selected = model.SelectedTenantIds.Contains(tenant.Id)
            }).ToList();
        }

        /// <summary>
        /// Prepare selected and all available tenants for the passed model by tenant mappings
        /// </summary>
        public async Task PrepareModelTenants<TModel, TEntity>(TModel model, TEntity entity, bool ignoreTenantMappings)
            where TModel : ITenantMappingSupportedModel 
            where TEntity : BaseEntity, ITenantMappingSupported
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ignoreTenantMappings && entity != null)
                model.SelectedTenantIds = _tenantMappingService.GetTenantsIdsWithAccess(entity).ToList();

            await PrepareModelTenants(model);
        }
    }
}