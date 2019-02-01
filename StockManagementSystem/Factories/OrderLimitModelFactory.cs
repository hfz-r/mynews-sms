using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.OrderLimits;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.OrderLimits;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Kendoui.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Factories
{
    /// <summary>
    /// Represents the order limit model factory implementation
    /// </summary>
    public class OrderLimitModelFactory : IOrderLimitModelFactory
    {
        private readonly IOrderLimitService _orderLimitService;
        private readonly IStoreService _storeService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public OrderLimitModelFactory(
            IOrderLimitService orderLimitService,
            IStoreService storeService,
            IDateTimeHelper dateTimeHelper)
        {
            _orderLimitService = orderLimitService;
            _storeService = storeService;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<OrderLimitSearchModel> PrepareOrderLimitSearchModel(OrderLimitSearchModel searchModel)
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

        public async Task<OrderLimitListModel> PrepareOrderLimitListModel(OrderLimitSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var orderLimits = await _orderLimitService.GetOrderLimitsAsync(
                storeIds: searchModel.SelectedStoreIds.ToArray(),
                percentage: searchModel.SearchPercentage,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new OrderLimitListModel
            {
                Data = orderLimits.Select(orderLimit =>
                {
                    var orderLimitsModel = orderLimit.ToModel<OrderLimitModel>();

                    orderLimitsModel.Percentage = orderLimit.Percentage;
                    orderLimitsModel.DaysofSales = orderLimit.DaysofSales;
                    orderLimitsModel.DaysofStock = orderLimit.DaysofStock;
                    orderLimitsModel.StoreName = String.Join(", ", orderLimit.OrderLimitStores.Select(store => store.Store.P_BranchNo + " - " + store.Store.P_Name));
                    orderLimitsModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(orderLimit.CreatedOnUtc, DateTimeKind.Utc);
                    orderLimitsModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(orderLimit.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);

                    return orderLimitsModel;
                }),
                Total = orderLimits.TotalCount
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

        public async Task<OrderLimitModel> PrepareOrderLimitListModel()
        {
            var orderLimits = await _orderLimitService.GetAllOrderLimitsAsync();

            var model = new OrderLimitModel
            {
                OrderLimits = orderLimits
            };

            return model;
        }

        public async Task<OrderLimitModel> PrepareOrderLimitModel(OrderLimitModel model, OrderLimit orderLimit)
        {
            if (orderLimit != null)
            {
                model = model ?? new OrderLimitModel();

                model.Id = orderLimit.Id;
                model.Percentage = orderLimit.Percentage;
                model.DaysofSales = orderLimit.DaysofSales;
                model.DaysofStock = orderLimit.DaysofStock;
                model.SelectedStoreIds = orderLimit.OrderLimitStores.Select(ols => ols.StoreId).ToList();
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(orderLimit.CreatedOnUtc, DateTimeKind.Utc);
                model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(orderLimit.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);
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