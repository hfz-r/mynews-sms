using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Replenishments;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Replenishments;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Kendoui.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Factories
{
    /// <summary>
    /// Represents the order limit model factory implementation
    /// </summary>
    public class ReplenishmentModelFactory : IReplenishmentModelFactory
    {
        private readonly IReplenishmentService _replenishmentService;
        private readonly IStoreService _storeService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public ReplenishmentModelFactory(
            IReplenishmentService replenishmentService,
            IStoreService storeService,
            IDateTimeHelper dateTimeHelper)
        {
            _replenishmentService = replenishmentService;
            _storeService = storeService;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<ReplenishmentSearchModel> PrepareReplenishmentSearchModel(ReplenishmentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            var stores = await _storeService.GetStoresAsync();
            searchModel.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo.ToString() + " - " + store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            return await Task.FromResult(searchModel);
        }

        public async Task<ReplenishmentListModel> PrepareReplenishmentListModel(ReplenishmentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var replenishments = await _replenishmentService.GetReplenishmentsAsync(
                storeIds: searchModel.SelectedStoreIds.ToArray(),
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new ReplenishmentListModel
            {
                Data = replenishments.Select(replenishment =>
                {
                    var replenishmentsModel = replenishment.ToModel<ReplenishmentModel>();

                    replenishmentsModel.BufferDays = replenishment.BufferDays;
                    replenishmentsModel.ReplenishmentQty = replenishment.ReplenishmentQty;
                    replenishmentsModel.StoreName = String.Join(", ", replenishment.ReplenishmentStores.Select(store => store.Store.P_BranchNo + " - " + store.Store.P_Name));
                    replenishmentsModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(replenishment.CreatedOnUtc, DateTimeKind.Utc);
                    replenishmentsModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(replenishment.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);

                    return replenishmentsModel;
                }),
                Total = replenishments.TotalCount
            };

            // sort
            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            // filter
            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        public async Task<ReplenishmentModel> PrepareReplenishmentListModel()
        {
            var replenishments = await _replenishmentService.GetAllReplenishmentsAsync();

            var model = new ReplenishmentModel
            {
                Replenishments = replenishments
            };

            return model;
        }

        public async Task<ReplenishmentModel> PrepareReplenishmentModel(ReplenishmentModel model, Replenishment replenishment)
        {
            if (replenishment != null)
            {
                model = model ?? new ReplenishmentModel();

                model.Id = replenishment.Id;
                model.BufferDays = replenishment.BufferDays;
                model.ReplenishmentQty = replenishment.ReplenishmentQty;
                model.SelectedStoreIds = replenishment.ReplenishmentStores.Select(ols => ols.StoreId).ToList();
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(replenishment.CreatedOnUtc, DateTimeKind.Utc);
                model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(replenishment.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);
            }

            var stores = await _storeService.GetStoresAsync();
            model.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo.ToString() + " - " + store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            return await Task.FromResult(model);
        }
    }
}