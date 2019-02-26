using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Tenants;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Factories
{
    public interface ITenantMappingSupportedModelFactory
    {
        Task PrepareModelTenants<TModel, TEntity>(TModel model, TEntity entity, bool ignoreTenantMappings)
            where TModel : ITenantMappingSupportedModel
            where TEntity : BaseEntity, ITenantMappingSupported;

        Task PrepareModelTenants<TModel>(TModel model) where TModel : ITenantMappingSupportedModel;
    }
}