using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Factories
{
    public interface IStoreMappingSupportedModelFactory
    {
        Task PrepareModelStores<TModel, TEntity>(TModel model, TEntity entity, bool ignoreStoreMappings)
            where TModel : IStoreMappingSupportedModel
            where TEntity : BaseEntity, IStoreMappingSupported;

        Task PrepareModelStores<TModel>(TModel model) where TModel : IStoreMappingSupportedModel;
    }
}