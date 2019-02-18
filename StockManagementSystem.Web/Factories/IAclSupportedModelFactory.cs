using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Factories
{
    public interface IAclSupportedModelFactory
    {
        void PrepareModelRoles<TModel>(TModel model) where TModel : IAclSupportedModel;

        Task PrepareModelRoles<TModel, TEntity>(TModel model, TEntity entity, bool ignorePermissionMappings)
            where TModel : IAclSupportedModel where TEntity : BaseEntity, IAclSupported;
    }
}