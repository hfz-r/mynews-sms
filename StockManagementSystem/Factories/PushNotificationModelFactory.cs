using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.PushNotifications;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.PushNotifications;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Kendoui.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static StockManagementSystem.Models.PushNotifications.PushNotificationModel;

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
        private readonly IConfiguration _iconfiguration;

        public PushNotificationModelFactory(
            IPushNotificationService pushNotificationService,
            IStoreService storeService,
            IConfiguration iconfiguration,
            IDateTimeHelper dateTimeHelper)
        {
            _pushNotificationService = pushNotificationService;
            _storeService = storeService;
            _iconfiguration = iconfiguration;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<PushNotificationSearchModel> PreparePushNotificationSearchModel(PushNotificationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            var stores = await _storeService.GetStores();
            searchModel.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo.ToString() + " - " + store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            var notificationCategories = await _pushNotificationService.GetNotificationCategoriesAsync();

            //TODO
            //if (notificationCategories != null && notificationCategories.Count > 0)
            //{
            //    _pushNotificationService.
            //}
            //else
            //{
            //}

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
                pushCategoryIds: searchModel.SelectedNotificationCategoryIds.ToArray(),
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
                    pushNotificationsModel.CategoryName = pushNotification.NotificationCategory.Name;

                    if (!string.IsNullOrEmpty(pushNotificationsModel.StockTakeNo))
                    {
                        pushNotificationsModel.StoreName = GetStockTakeStore(pushNotificationsModel.StockTakeNo);
                        pushNotificationsModel.CategoryName += " (" + pushNotificationsModel.StockTakeNo + ")";
                    }
                    else
                    {
                        pushNotificationsModel.StoreName = String.Join(", ", pushNotification.PushNotificationStores.Select(store => store.Store.P_BranchNo + " - " + store.Store.P_Name));
                    }
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

        private string GetStockTakeStore(string stockTakeNo)
        {
            string conString = ConfigurationExtensions.GetConnectionString(this._iconfiguration, "HQ");

            var model = new PushNotificationModel();
            List<StockTakeHeader> stockTakeList = new List<StockTakeHeader>();
            List<StoreList> storeList = new List<StoreList>();

            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();
                string sSQL = "SELECT  STH.Stock_Take_No, STO.Outlet_No, O.Outlet_Name FROM [dbo].[btb_HHT_StockTakeHeader] STH ";
                sSQL += "INNER JOIN [dbo].[btb_HHT_StockTakeOutlet] STO ";
                sSQL += "ON STH.Stock_Take_No = STO.Stock_Take_No ";
                sSQL += "INNER JOIN [dbo].[btb_HHT_Outlet] O ";
                sSQL += "ON STO.Outlet_No = O.Outlet_No ";
                sSQL += " WHERE [End_Date] >= GETDATE() ";

                if (!string.IsNullOrEmpty(stockTakeNo))
                {
                    sSQL += "AND STH.Stock_Take_No = '" + stockTakeNo + "'";
                }

                using (SqlCommand command = new SqlCommand(sSQL, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var stNo = reader["Stock_Take_No"].ToString();
                        var storeNo = reader["Outlet_No"].ToString();
                        var storeName = reader["Outlet_Name"].ToString();

                        if (stockTakeList != null & stockTakeList.Count > 0)
                        {
                            var item = stockTakeList.Any(x => x.StockTakeNo == stNo);
                            if (!item)
                            {
                                storeList.Add(new StoreList()
                                {
                                    StoreName = storeName,
                                    StoreNo = storeNo
                                });

                                stockTakeList.Add(new StockTakeHeader()
                                {
                                    StockTakeNo = stNo,
                                    Stores = storeList
                                });
                            }
                            else
                            {
                                foreach (var child in stockTakeList)
                                {
                                    if (child.StockTakeNo == stNo)
                                    {
                                        child.Stores.Add(new StoreList()
                                        {
                                            StoreName = storeName,
                                            StoreNo = storeNo
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            storeList.Add(new StoreList()
                            {
                                StoreName = storeName,
                                StoreNo = storeNo
                            });

                            stockTakeList.Add(new StockTakeHeader()
                            {
                                StockTakeNo = stNo,
                                Stores = storeList
                            });
                        }
                    }
                }
                connection.Close();
            }

            string storeNames = string.Empty;

            foreach (var parent in stockTakeList)
            {
                foreach (var child in parent.Stores)
                {
                    storeNames += child.StoreNo + " - " + child.StoreName;
                    storeNames += ", ";
                }
            }

            return storeNames.Substring(0, storeNames.LastIndexOf(","));
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
            model = model ?? new PushNotificationModel();

            if (pushNotification != null)
            {
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
            else
            {
                pushNotification = new PushNotification();
            }

            var stores = await _storeService.GetStores();
            model.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo.ToString() + " - " + store.P_Name,
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