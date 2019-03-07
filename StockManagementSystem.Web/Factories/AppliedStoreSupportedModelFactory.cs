using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Factories
{
    /// <summary>
    /// Represents the base store supported model factory implementation
    /// </summary>
    public partial class AppliedStoreSupportedModelFactory : IAppliedStoreSupportedModelFactory
    {
        /// <summary>
        /// Prepare selected and all available stores for the passed model
        /// </summary>
        /// <typeparam name="TModel">Store supported model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="availableStores">List of all available stores</param>
        public virtual void PrepareModelAppliedStores<TModel>(TModel model, IList<Store> availableStores)
            where TModel : IAppliedStoreSupportedModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvailableStores = availableStores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo + " - " + store.P_Name,
                Value = store.P_BranchNo.ToString(),
                Selected = model.SelectedStoreIds.Contains(store.P_BranchNo)
            }).ToList();
        }

        /// <summary>
        /// Prepare selected and all available stores for the passed model by entity applied stores
        /// </summary>
        /// <typeparam name="TModel">Store supported model type</typeparam>
        /// <typeparam name="TEntity">Store supported entity type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="entity">Entity</param>
        /// <param name="availableStores">List of all available stores</param>
        /// <param name="ignoreAppliedStores">Whether to ignore existing applied stores</param>
        /// <returns></returns>
        public virtual void PrepareModelAppliedStores<TModel, TEntity>(TModel model, TEntity entity, IList<Store> availableStores, bool ignoreAppliedStores) 
            where TModel : IAppliedStoreSupportedModel
            where TEntity : BaseEntity, IAppliedStoreSupported
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare already applied stores
            if (!ignoreAppliedStores && entity != null)
                model.SelectedStoreIds = entity.AppliedStores.Select(store => store.P_BranchNo).ToList();

            PrepareModelAppliedStores(model, availableStores);
        }
    }
}