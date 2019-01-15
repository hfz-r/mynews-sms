using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.PushNotifications;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.PushNotifications;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Kendoui.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Factories
{
    /// <summary>
    /// Represents the push notification model factory implementation
    /// </summary>
    public class PushNotificationModelFactory : IPushNotificationModelFactory
    {
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IStoreService _storeService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public PushNotificationModelFactory(
            IPushNotificationService pushNotificationService,
            IStoreService storeService,
            IDateTimeHelper dateTimeHelper)
        {
            _pushNotificationService = pushNotificationService;
            _storeService = storeService;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<PushNotificationSearchModel> PreparePushNotificationSearchModel(PushNotificationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            var stores = await _storeService.GetStoresAsync();
            searchModel.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();
            
            var notificationCategories = await _pushNotificationService.GetNotificationCategoriesAsync();
            searchModel.AvailableNotificationCategory = notificationCategories.Select(notificationCategory => new SelectListItem
            {
                Text = notificationCategory.Name,
                Value = notificationCategory.Id.ToString()
            }).ToList();

            return await Task.FromResult(searchModel);
        }

        public async Task<PushNotificationListModel> PreparePushNotificationListModel(PushNotificationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var pushNotifications = await _pushNotificationService.GetPushNotificationsAsync(
                storeIds: searchModel.SelectedStoreIds.ToArray(),
                title: searchModel.SearchTitle,
                desc: searchModel.SearchDesc,
                stNo: searchModel.SearchStockTakeNo,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new PushNotificationListModel
            {
                Data = pushNotifications.Select(pushNotification =>
                {
                    var pushNotificationsModel = pushNotification.ToModel<PushNotificationModel>();

                    pushNotificationsModel.Title = pushNotification.Title;
                    pushNotificationsModel.Description = pushNotification.Desc;
                    pushNotificationsModel.StockTakeNo = pushNotification.StockTakeNo;
                    pushNotificationsModel.StoreName = String.Join(", ", pushNotification.PushNotificationStores.Select(store => store.Store.P_Name));
                    pushNotificationsModel.CategoryName = String.Join(", ", pushNotification.NotificationCategory.Name);
                    pushNotificationsModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(pushNotification.CreatedOnUtc, DateTimeKind.Utc);
                    pushNotificationsModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(pushNotification.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);

                    return pushNotificationsModel;
                }),
                Total = pushNotifications.TotalCount
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

        public async Task<PushNotificationModel> PreparePushNotificationListModel()
        {
            var pushNotifications = await _pushNotificationService.GetAllPushNotificationsAsync();

            var model = new PushNotificationModel
            {
                PushNotifications = pushNotifications
            };

            return model;
        }

        public async Task<PushNotificationModel> PreparePushNotificationModel(PushNotificationModel model, PushNotification pushNotification)
        {
            if (pushNotification != null)
            {
                model = model ?? new PushNotificationModel();

                model.Id = pushNotification.Id;
                model.Title = pushNotification.Title;
                model.Description = pushNotification.Desc;
                model.StockTakeNo = pushNotification.StockTakeNo;
                model.CategoryName = pushNotification.NotificationCategory.Name;
                List<int> catIds = new List<int>
                {
                    pushNotification.NotificationCategoryId
                };
                model.SelectedNotificationCategoryIds = catIds;
                model.SelectedStoreIds = pushNotification.PushNotificationStores.Select(pns => pns.StoreId).ToList();
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(pushNotification.CreatedOnUtc, DateTimeKind.Utc);
                model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(pushNotification.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);
            }

            var stores = await _storeService.GetStoresAsync();
            model.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            var categories = await _pushNotificationService.GetNotificationCategoriesAsync();
            model.AvailableNotificationCategories = categories.Select(category => new SelectListItem
            {
                Text = category.Name,
                Value = category.Id.ToString()
            }).ToList();

            return await Task.FromResult(model);
        }
    }
}