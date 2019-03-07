using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.PushNotifications;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.PushNotifications;
using StockManagementSystem.Services.Scheduler;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static StockManagementSystem.Models.PushNotifications.PushNotificationModel;
using Newtonsoft.Json;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Controllers
{
    public class PushNotificationController : BaseController
    {
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IStoreService _storeService;
        private readonly IRepository<PushNotification> _pushNotificationRepository;
        private readonly IRepository<PushNotificationStore> _pushNotificationStoreRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IDeviceModelFactory _deviceModelFactory;
        private readonly IPushNotificationModelFactory _pushNotificationModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _iconfiguration;
        private readonly ILogger _logger;

        #region Constructor

        public PushNotificationController(
            IPushNotificationService pushNotificationService,
            IStoreService storeService,
            IRepository<PushNotification> pushNotificationRepository,
            IRepository<PushNotificationStore> pushNotificationStoreRepository,
            IRepository<Store> storeRepository,
            IRepository<Device> deviceRepository,
            IDeviceModelFactory deviceModelFactory,
            IPushNotificationModelFactory pushNotificationModelFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            IConfiguration iconfiguration,
            ILoggerFactory loggerFactory)
        {
            this._pushNotificationService = pushNotificationService;
            this._storeService = storeService;
            this._pushNotificationRepository = pushNotificationRepository;
            this._pushNotificationStoreRepository = pushNotificationStoreRepository;
            this._storeRepository = storeRepository;
            this._deviceModelFactory = deviceModelFactory;
            this._pushNotificationModelFactory = pushNotificationModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _iconfiguration = iconfiguration;
            _logger = loggerFactory.CreateLogger<PushNotificationController>();
        }

        public ILogger Logger { get; }

        #endregion

        public async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
                return AccessDeniedView();

            var model = await _pushNotificationModelFactory.PreparePushNotificationSearchModel(new PushNotificationSearchModel());

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> AddNotification(PushNotificationModel model)
        {

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
                return AccessDeniedView();

            PushNotification pushNotification = new PushNotification();

            //validate stores
            var allStores = await _storeService.GetStores();
            var newStores = new List<Store>();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                    newStores.Add(store);
            }

            if (model.SelectedNotificationCategoryIds.Count() == 0)
            {
                _notificationService.ErrorNotification("Category is required");
                model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                model.SelectedNotificationCategoryIds = new List<int>();

                return View(model);
            }
            else
            {
                if (model.SelectedNotificationCategoryIds.FirstOrDefault() == 1)
                {
                    model.StockTakeList = GetStockTakeStore(string.Empty);
                    model.AvailableStockTakeList = model.StockTakeList.Select(stList => new SelectListItem
                    {
                        Text = stList.StockTakeNo,
                        Value = stList.StockTakeNo
                    }).ToList();

                    if (model.AvailableStockTakeList.Count > 0 && model.SelectedStockTake == null)
                    {
                        _notificationService.ErrorNotification("Stock Take # is required");
                        model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                        model.StockTakeNo = string.Empty;

                        return View(model);
                    }
                    else
                    {
                        model.StockTakeNo = model.SelectedStockTake.FirstOrDefault().ToString();
                    }
                }
                else
                {
                    model.StockTakeNo = string.Empty;

                    if (model.SelectedStoreIds.Count == 0)
                    {
                        _notificationService.ErrorNotification("Store is required");
                        model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                        model.SelectedStoreIds = new List<int>();

                        return View(model);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pushNotification.Title = model.Title;
                    pushNotification.Desc = model.Description;
                    pushNotification.StockTakeNo = string.IsNullOrEmpty(model.StockTakeNo) ? string.Empty : model.StockTakeNo;
                    pushNotification.NotificationCategoryId = model.SelectedNotificationCategoryIds.FirstOrDefault();

                    if (pushNotification.NotificationCategoryId != 1)
                    {
                        //stores
                        List<PushNotificationStore> pushNotificationStoreList = new List<PushNotificationStore>();
                        pushNotification.PushNotificationStores = new List<PushNotificationStore>();

                        foreach (var store in allStores)
                        {
                            if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                            {
                                PushNotificationStore pushNotificationStore = new PushNotificationStore();
                                pushNotificationStore.PushNotificationId = pushNotification.Id;
                                pushNotificationStore.StoreId = store.P_BranchNo;
                                pushNotification.PushNotificationStores.Add(pushNotificationStore);
                            }
                        }
                    }

                    _pushNotificationService.UpdatePushNotification(pushNotification);

                    _notificationService.SuccessNotification("Push Notification has been added successfully.");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }
            }

            model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);

            return View(model);

            //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
            //    return AccessDeniedView();

            //if (model.SelectedStoreIds.Count == 0)
            //{
            //    ModelState.AddModelError(string.Empty, "Store is required to create push notification");
            //    _notificationService.ErrorNotification("Store is required to create push notification");
            //    return new NullJsonResult();
            //}

            //if (model.SelectedNotificationCategoryIds.Count == 0)
            //{
            //    ModelState.AddModelError(string.Empty, "Notification Category is required to create push notification");
            //    _notificationService.ErrorNotification("Notification Category is required to create push notification");
            //    return new NullJsonResult();
            //}

            //try
            //{
            //    PushNotification pushNotification = new PushNotification();
            //    pushNotification.Title = model.Title;
            //    pushNotification.Desc = model.Description;
            //    pushNotification.StockTakeNo = model.SelectedNotificationCategoryIds.FirstOrDefault() == 1 ? model.StockTakeNo : string.Empty;
            //    pushNotification.NotificationCategoryId = model.SelectedNotificationCategoryIds.FirstOrDefault();
            //    pushNotification.PushNotificationStores = new List<PushNotificationStore>();

            //    //Add store
            //    foreach (var store in model.SelectedStoreIds)
            //    {
            //        PushNotificationStore pushNotificationStore = new PushNotificationStore();
            //        pushNotificationStore.PushNotificationId = pushNotification.Id;
            //        pushNotificationStore.StoreId = store;

            //        pushNotification.PushNotificationStores.Add(pushNotificationStore);
            //    }

            //    await _pushNotificationService.InsertPushNotification(pushNotification);

            //    //if successful, only for stock take
            //    if (model.SelectedNotificationCategoryIds.FirstOrDefault() == 1)
            //    {
            //        foreach (var item in model.SelectedStoreIds)
            //        {
            //            var result = _deviceModelFactory.PrepareDeviceListbyStoreModel(item);

            //            if (result.Result != null && result.Result.Devices != null)
            //            {
            //                if (result.Result.Devices.Count > 0)
            //                {
            //                    foreach (var devices in result.Result.Devices)
            //                    {
            //                        if (!string.IsNullOrEmpty(devices.TokenId))
            //                        {
            //                            Scheduler.TriggerScheduler(model.Title,
            //                                model.Description,
            //                                devices.TokenId,
            //                                _iconfiguration["FirebaseServerKey"].ToString(),
            //                                _iconfiguration["FirebaseSenderID"].ToString());
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    return new NullJsonResult();
            //}
            //catch (Exception e)
            //{
            //    ModelState.AddModelError(string.Empty, e.Message);
            //    _notificationService.ErrorNotification(e.Message);

            //    return Json(e.Message);
            //}
        }

        public async Task<IActionResult> AddNotification()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
                return AccessDeniedView();

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
                return AccessDeniedView();


            var model = await _pushNotificationModelFactory.PreparePushNotificationModel(null, null);
            model.StockTakeList = GetStockTakeStore(string.Empty);

            model.AvailableStockTakeList = model.StockTakeList.Select(stList => new SelectListItem
            {
                Text = stList.StockTakeNo,
                Value = stList.StockTakeNo
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PushNotificationList(PushNotificationSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
                return AccessDeniedKendoGridJson();

            var model = await _pushNotificationModelFactory.PreparePushNotificationListModel(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> EditNotification(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
                return AccessDeniedView();

            var pushNotification = await _pushNotificationService.GetPushNotificationByIdAsync(id);
            if (pushNotification == null)
                return RedirectToAction("Index");

            var model = await _pushNotificationModelFactory.PreparePushNotificationModel(null, pushNotification);
            model.StockTakeList = GetStockTakeStore(string.Empty);

            model.AvailableStockTakeList = model.StockTakeList.Select(stList => new SelectListItem
            {
                Text = stList.StockTakeNo,
                Value = stList.StockTakeNo
            }).ToList();


            if (!string.IsNullOrEmpty(model.StockTakeNo))
            {
                model.SelectedStockTake = new List<int?>();
                model.SelectedStockTake.Add(Convert.ToInt32(model.StockTakeNo));
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> EditNotification(PushNotificationModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
                return AccessDeniedView();

            var pushNotification = await _pushNotificationService.GetPushNotificationByIdAsync(model.Id);
            if (pushNotification == null)
                return RedirectToAction("Index");

            //validate stores
            var allStores = await _storeService.GetStores();
            var newStores = new List<Store>();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                    newStores.Add(store);
            }

            if (model.SelectedNotificationCategoryIds.Count() == 0)
            {
                _notificationService.ErrorNotification("Category is required");
                model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                model.SelectedNotificationCategoryIds = new List<int>();

                return View(model);
            }
            else
            {
                if (model.SelectedNotificationCategoryIds.FirstOrDefault() == 1)
                {
                    model.StockTakeList = GetStockTakeStore(string.Empty);
                    model.AvailableStockTakeList = model.StockTakeList.Select(stList => new SelectListItem
                    {
                        Text = stList.StockTakeNo,
                        Value = stList.StockTakeNo
                    }).ToList();

                    if (model.AvailableStockTakeList.Count > 0 && model.SelectedStockTake == null)
                    {
                        _notificationService.ErrorNotification("Stock Take # is required");
                        model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                        model.StockTakeNo = string.Empty;

                        return View(model);
                    }
                    else
                    {
                        model.StockTakeNo = model.SelectedStockTake.FirstOrDefault().ToString();
                    }
                }
                else
                {
                    model.StockTakeNo = string.Empty;

                    if (model.SelectedStoreIds.Count == 0)
                    {
                        _notificationService.ErrorNotification("Store is required");
                        model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                        model.SelectedStoreIds = new List<int>();

                        return View(model);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pushNotification.Title = model.Title;
                    pushNotification.Desc = model.Description;
                    pushNotification.StockTakeNo = string.IsNullOrEmpty(model.StockTakeNo) ? string.Empty : model.StockTakeNo;
                    pushNotification.NotificationCategoryId = model.SelectedNotificationCategoryIds.FirstOrDefault();

                    //stores
                    List<PushNotificationStore> pushNotificationStoreList = new List<PushNotificationStore>();

                    foreach (var store in allStores)
                    {
                        if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                        {
                            //new store
                            if (pushNotification.PushNotificationStores.Count(mapping => mapping.StoreId == store.P_BranchNo) == 0)
                            {
                                PushNotificationStore pushNotificationStore = new PushNotificationStore();
                                pushNotificationStore.PushNotificationId = pushNotification.Id;
                                pushNotificationStore.StoreId = store.P_BranchNo;

                                pushNotification.PushNotificationStores.Add(pushNotificationStore);
                            }
                        }
                        else
                        {
                            //remove store
                            if (pushNotification.PushNotificationStores.Count(mapping => mapping.StoreId == store.P_BranchNo) > 0)
                                _pushNotificationService.DeletePushNotificationStore(model.Id, store);
                        }
                    }

                    _pushNotificationService.UpdatePushNotification(pushNotification);

                    _notificationService.SuccessNotification("Push Notification has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("Index");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("EditNotification", new { id = pushNotification.Id });
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }
            }

            model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);

            return View(model);
        }

        public async Task<IActionResult> DeleteNotification(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
                return AccessDeniedView();

            var pushNotification = await _pushNotificationService.GetPushNotificationByIdAsync(id);
            if (pushNotification == null)
                return RedirectToAction("Index");

            try
            {
                _pushNotificationService.DeletePushNotification(pushNotification);

                _notificationService.SuccessNotification("Push Notification has been deleted successfully.");

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return RedirectToAction("EditNotification", new { id = pushNotification.Id });
            }
        }

        private void AddErrors(string result)
        {
            ModelState.AddModelError(string.Empty, result);
        }

        private List<StockTakeHeader> GetStockTakeStore(string stockTakeNo)
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
            return stockTakeList;
        }

        [HttpGet]
        public ActionResult GetStockTakeStores(string StockTakeNo)
        {
            var StockTakeStores = new List<string>();
            List<StockTakeHeader> stList = GetStockTakeStore(StockTakeNo);
            string storeNames = string.Empty;
            foreach (var parent in stList)
            {
                foreach (var child in parent.Stores)
                {
                    storeNames += child.StoreNo + " - " + child.StoreName;
                    storeNames += ", ";
                }
            }

            return Json(storeNames.Substring(0, storeNames.LastIndexOf(",")));
        }
    }
}