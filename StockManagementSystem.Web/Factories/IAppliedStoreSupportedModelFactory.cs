using System.Collections.Generic;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Factories
{
    public interface IAppliedStoreSupportedModelFactory
    {
        void PrepareModelAppliedStores<TModel, TEntity>(TModel model, TEntity entity, IList<Store> availableStores, bool ignoreAppliedStores)
            where TModel : IAppliedStoreSupportedModel
            where TEntity : BaseEntity, IAppliedStoreSupported;

        void PrepareModelAppliedStores<TModel>(TModel model, IList<Store> availableStores) where TModel : IAppliedStoreSupportedModel;
    }
}