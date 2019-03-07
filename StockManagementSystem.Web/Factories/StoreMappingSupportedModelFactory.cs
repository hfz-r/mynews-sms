using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Factories
{
    public class StoreMappingSupportedModelFactory : IStoreMappingSupportedModelFactory
    {
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

        public StoreMappingSupportedModelFactory(
            IStoreMappingService storeMappingService, 
            IStoreService storeService)
        {
            _storeMappingService = storeMappingService;
            _storeService = storeService;
        }

        /// <summary>
        /// Prepare selected and all available stores for the passed model
        /// </summary>
        public async Task PrepareModelStores<TModel>(TModel model) where TModel : IStoreMappingSupportedModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var availableStores = await _storeService.GetStores();
            model.AvailableStores = availableStores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo.ToString() + " - " + store.P_Name,
                Value = store.P_BranchNo.ToString(),
                Selected = model.SelectedStoreIds.Contains(store.P_BranchNo)
            }).ToList();
        }

        /// <summary>
        /// Prepare selected and all available stores for the passed model by store mappings
        /// </summary>
        public async Task PrepareModelStores<TModel, TEntity>(TModel model, TEntity entity, bool ignoreStoreMappings)
            where TModel : IStoreMappingSupportedModel 
            where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ignoreStoreMappings && entity != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(entity).ToList();

            await PrepareModelStores(model);
        }
    }
}