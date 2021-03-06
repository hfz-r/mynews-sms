﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
using StockManagementSystem.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Data;
using System.Reflection;
using System.ComponentModel;
using Quartz;
using Quartz.Impl;
using static StockManagementSystem.Models.PushNotifications.PushNotificationModel;

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

            #region Validation

            //validate stores
            var allStores = await _storeService.GetStores();
            var newStores = new List<Store>();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                    newStores.Add(store);
            }

            if (!model.SelectedNotificationCategoryIds.Any())
            {
                _notificationService.ErrorNotification("Category is required");
                model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                model = GetStore(model);
                model = GetRepeat(model);
                model.SelectedNotificationCategoryIds = new List<int?>();

                return View(model);
            }
            else if (model.SelectedNotificationCategoryIds.Count() == 1)
            {
                var catId = model.SelectedNotificationCategoryIds.FirstOrDefault();
                if (catId == null)
                {
                    _notificationService.ErrorNotification("Category is required");
                    model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                    model = GetStore(model);
                    model = GetRepeat(model);
                    model.SelectedNotificationCategoryIds = new List<int?>();

                    return View(model);
                }
            }

            if (model.SelectedNotificationCategoryIds.FirstOrDefault() == 1)
            {
                if (model.SelectedStockTake == null)
                {
                    _notificationService.ErrorNotification("Stock Take # is required");
                    model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                    model = GetStore(model);
                    model = GetRepeat(model);
                    model.StockTakeNo = null;

                    return View(model);
                }
                else if (model.SelectedStockTake != null && model.SelectedStockTake.Count() == 1)
                {
                    if (model.SelectedStockTake.FirstOrDefault() == null)
                    {
                        _notificationService.ErrorNotification("Stock Take # is required");
                        model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                        model = GetStore(model);
                        model = GetRepeat(model);
                        model.StockTakeNo = null;

                        return View(model);
                    }
                    else
                    {
                        model.StockTakeNo = model.SelectedStockTake.FirstOrDefault();
                    }
                }
                else
                {
                    model.StockTakeNo = model.SelectedStockTake.FirstOrDefault();
                }
            }
            else
            {
                model.StockTakeNo = null;

                if (model.SelectedStoreIds.Count == 0)
                {
                    _notificationService.ErrorNotification("Store is required");
                    model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                    model.SelectedStoreIds = new List<int>();

                    return View(model);
                }
            }

            #endregion  

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime currentTime = DateTime.Now;

                    pushNotification.Title = model.Title;
                    pushNotification.Desc = model.Description;
                    pushNotification.StockTakeNo = model.StockTakeNo == null ? 0 : model.StockTakeNo;
                    pushNotification.NotificationCategoryId = Convert.ToInt32(model.SelectedNotificationCategoryIds.FirstOrDefault());
                    pushNotification.Interval = model.RemindMe ? model.SelectedRepeat : null;
                    pushNotification.RemindMe = model.RemindMe;
                    pushNotification.StartTime = model.RemindMe ? model.StartTime : null;
                    pushNotification.EndTime = model.RemindMe ? model.EndTime : null;

                    if (model.SelectedNotificationCategoryIds.FirstOrDefault() == 1)
                    {
                        model.SelectedStoreIds = new List<int>();
                        List<StockTakeHeader> stList = GetStockTakeStore(model.StockTakeNo);
                        foreach (var parent in stList)
                        {
                            foreach (var child in parent.Stores)
                            {
                                model.SelectedStoreIds.Add(Convert.ToInt32(child.StoreNo));
                            }
                        }
                    }

                    //stores
                    List<PushNotificationStore> pushNotificationStoreList = new List<PushNotificationStore>();
                    pushNotification.PushNotificationStores = new List<PushNotificationStore>();

                    List<PushNotificationDevice> pushNotificationDeviceList = new List<PushNotificationDevice>();
                    pushNotification.PushNotificationDevices = new List<PushNotificationDevice>();
                    int i = 0;

                    foreach (var item in model.SelectedStoreIds)
                    {
                        if (allStores.Any(x => x.P_BranchNo == item))
                        {
                            i = 0;
                            PushNotificationStore pushNotificationStore =
                                new PushNotificationStore
                                {
                                    PushNotificationId = pushNotification.Id,
                                    StoreId = item
                                };
                            pushNotification.PushNotificationStores.Add(pushNotificationStore);

                            //device
                            var deviceResult = _deviceModelFactory.PrepareDeviceListbyStoreModel(item);

                            if (deviceResult.Result?.Devices?.Count > 0)
                            {
                                foreach (var devices in deviceResult.Result.Devices)
                                {
                                    i++;
                                    PushNotificationDevice pushNotificationDevice =
                                        new PushNotificationDevice
                                        {
                                            PushNotificationId = pushNotification.Id,
                                            JobName = model.RemindMe ? "Job" + currentTime.ToString("ddMMyyyyHHmmssfff") + i : null,
                                            JobGroup = model.RemindMe ? "Group" + currentTime.ToString("ddMMyyyyHHmmssfff") + i : null,
                                            DeviceId = devices.Id
                                        };
                                    pushNotification.PushNotificationDevices.Add(pushNotificationDevice);
                                }
                            }
                        }
                        else if (item == -99) //All stores
                        {
                            i = 0;
                            foreach(var store in allStores)
                            {
                                PushNotificationStore pushNotificationStore =
                                    new PushNotificationStore
                                    {
                                        PushNotificationId = pushNotification.Id,
                                        StoreId = store.P_BranchNo
                                    };
                                pushNotification.PushNotificationStores.Add(pushNotificationStore);
                            }
                            //device
                            var deviceResult = _deviceModelFactory.PrepareDeviceListbyStoreModel(item);

                            if (deviceResult.Result?.Devices?.Count > 0)
                            {
                                foreach (var devices in deviceResult.Result.Devices)
                                {
                                    i++;
                                    PushNotificationDevice pushNotificationDevice =
                                        new PushNotificationDevice
                                        {
                                            PushNotificationId = pushNotification.Id,
                                            JobName = model.RemindMe ? "Job" + currentTime.ToString("ddMMyyyyHHmmssfff") + i : null,
                                            JobGroup = model.RemindMe ? "Group" + currentTime.ToString("ddMMyyyyHHmmssfff") + i : null,
                                            DeviceId = devices.Id
                                        };
                                    pushNotification.PushNotificationDevices.Add(pushNotificationDevice);
                                }
                            }
                        }
                    }

                    await _pushNotificationService.InsertPushNotification(pushNotification);

                    _notificationService.SuccessNotification("Push Notification has been added successfully.");

                    //selected tab
                    SaveSelectedTabName();

                    //Others
                    if (model.RemindMe)
                    {
                        string title = string.Empty;
                        if (pushNotification.StockTakeNo != 0)
                        {
                            title = model.Title + "(Stock Take #" + pushNotification.StockTakeNo + ")";
                        }
                        else
                        {
                            title = model.Title;
                        }

                        foreach (var item in pushNotification.PushNotificationDevices)
                        {
                            if (item.Device != null && !string.IsNullOrEmpty(item.Device.TokenId))
                            {
                                //why not use IScheduledTask? this is repetitive = performance.
                                Scheduler.Start(
                                    item.JobName,
                                    item.JobGroup,
                                    model.Title,
                                    model.Description,
                                    item.Device.TokenId,
                                    model.StartTime.GetValueOrDefault(),
                                    model.EndTime.GetValueOrDefault().Add(new TimeSpan(23, 59, 59)),
                                    model.SelectedRepeat,
                                    _iconfiguration["APIKey"].ToString()
                                    /*,_iconfiguration["FirebaseSenderID"].ToString()*/); //not required
                            }
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }
            }

            model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);

            return View(model);
        }

        private PushNotificationModel GetStore(PushNotificationModel model)
        {
            model.StockTakeList = GetStockTakeStore(null);
            model.AvailableStockTakeList = model.StockTakeList.Select(stList => new SelectListItem
            {
                Text = stList.StockTakeNo,
                Value = stList.StockTakeNo
            }).ToList();

            model.SelectedStockTake = new List<int?>();

            return model;
        }

        private PushNotificationModel GetRepeat(PushNotificationModel model)
        {
            Array arr = Enum.GetValues(typeof(RepeatEnum));
            List<string> lstDays = new List<string>(arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                model.AvailableRepeatList.Add(new SelectListItem
                {
                    Text = GetDescription((RepeatEnum)Enum.Parse(typeof(RepeatEnum), arr.GetValue(i).ToString())),
                    Value = ((int)arr.GetValue(i)).ToString()
                });
            }

            model.SelectedRepeat = new int?();

            return model;
        }

        public async Task<IActionResult> AddNotification()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
                return AccessDeniedView();

            var model = await _pushNotificationModelFactory.PreparePushNotificationModel(null, null);
            model.StockTakeList = GetStockTakeStore(null);

            model.AvailableStockTakeList = model.StockTakeList.Select(stList => new SelectListItem
            {
                Text = stList.StockTakeNo,
                Value = stList.StockTakeNo
            }).ToList();

            model.AvailableRepeatList = new List<SelectListItem>();

            model = GetRepeat(model);

            return View(model);
        }

        public static string GetDescription(Enum input)
        {
            Type type = input.GetType();
            MemberInfo[] memInfo = type.GetMember(input.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = (object[])memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return input.ToString();
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
            model.StockTakeList = GetStockTakeStore(null);

            model = GetRepeat(model);
            
            model.SelectedRepeat = pushNotification.Interval;
            model.StartTime = pushNotification.StartTime != null ? pushNotification.StartTime.Value : DateTime.Now;
            model.EndTime = pushNotification.EndTime != null ? pushNotification.EndTime.Value : DateTime.Now;
            
            model.AvailableStockTakeList = model.StockTakeList.Select(stList => new SelectListItem
            {
                Text = stList.StockTakeNo,
                Value = stList.StockTakeNo
            }).ToList();

            if (model.StockTakeNo != 0)
            {
                model.SelectedStockTake = new List<int?>();
                model.SelectedStockTake.Add(model.StockTakeNo);
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

            #region Validation

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
                model.SelectedNotificationCategoryIds = new List<int?>();

                return View(model);
            }
            else
            {
                if (model.SelectedNotificationCategoryIds.FirstOrDefault() == 1)
                {
                    model.StockTakeList = GetStockTakeStore(null);
                    model.AvailableStockTakeList = model.StockTakeList.Select(stList => new SelectListItem
                    {
                        Text = stList.StockTakeNo,
                        Value = stList.StockTakeNo
                    }).ToList();

                    if (model.AvailableStockTakeList.Count > 0 && model.SelectedStockTake == null)
                    {
                        _notificationService.ErrorNotification("Stock Take # is required");
                        model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                        model.StockTakeNo = null;

                        return View(model);
                    }

                    model.StockTakeNo = model.SelectedStockTake.FirstOrDefault();
                }
                else
                {
                    model.StockTakeNo = null;

                    if (model.SelectedStoreIds.Count == 0)
                    {
                        _notificationService.ErrorNotification("Store is required");
                        model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                        model.SelectedStoreIds = new List<int>();

                        return View(model);
                    }
                }
            }

            #endregion 

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime currentTime = DateTime.Now;

                    //Delete existing job and recreate later
                    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
                    if (pushNotification.RemindMe)
                    {
                        if (pushNotification.PushNotificationDevices != null)
                        {
                            foreach (var item in pushNotification.PushNotificationDevices)
                            {
                                if (await scheduler.CheckExists(new JobKey(item.JobName, item.JobGroup)))
                                    await scheduler.DeleteJob(new JobKey(item.JobName, item.JobGroup));
                            }
                        }

                    }

                    pushNotification.Title = model.Title;
                    pushNotification.Desc = model.Description;
                    pushNotification.StockTakeNo = model.StockTakeNo == null ? 0 : model.StockTakeNo;
                    pushNotification.NotificationCategoryId = (int)model.SelectedNotificationCategoryIds.FirstOrDefault();
                    pushNotification.RemindMe = model.RemindMe;
                    pushNotification.Interval = model.RemindMe ? model.SelectedRepeat : null;
                    pushNotification.StartTime = model.RemindMe ? model.StartTime : null;
                    pushNotification.EndTime = model.RemindMe ? model.EndTime : null;

                    if (model.SelectedNotificationCategoryIds.FirstOrDefault() == 1)
                    {
                        model.SelectedStoreIds = new List<int>();
                        List<StockTakeHeader> stList = GetStockTakeStore(model.StockTakeNo);
                        foreach (var parent in stList)
                        {
                            foreach (var child in parent.Stores)
                            {
                                model.SelectedStoreIds.Add(Convert.ToInt32(child.StoreNo));
                            }
                        }
                    }

                    //stores
                    List<PushNotificationStore> pushNotificationStoreList = new List<PushNotificationStore>();
                    List<PushNotificationDevice> pushNotificationDeviceList = new List<PushNotificationDevice>();
                    List<Tuple<int, string>> deleteList = new List<Tuple<int, string>>();


                    int i = 0;

                    foreach (var item in pushNotification.PushNotificationStores)
                    {
                        if (!model.SelectedStoreIds.Any(x => x == item.StoreId))
                        {

                            // add an item
                            deleteList.Add(new Tuple<int, string>(model.Id, item.StoreId.ToString()));
                        }
                    }
                    if (deleteList != null && deleteList.Count > 0)
                    {
                        foreach (var deleteItem in deleteList)
                        {
                            //remove store
                            _pushNotificationService.DeletePushNotificationStore(deleteItem.Item1, Convert.ToInt32(deleteItem.Item2));
                        }
                    }

                    foreach (var item in model.SelectedStoreIds)
                    {
                        if (allStores.Any(x => x.P_BranchNo == item))
                        {
                            //new store
                            if (pushNotification.PushNotificationStores.Count(mapping => mapping.StoreId == item) == 0)
                            {
                                i = 0;
                                PushNotificationStore pushNotificationStore =
                                    new PushNotificationStore
                                    {
                                        PushNotificationId = pushNotification.Id,
                                        StoreId = item
                                    };

                                pushNotification.PushNotificationStores.Add(pushNotificationStore);

                                //device
                                var deviceResult = _deviceModelFactory.PrepareDeviceListbyStoreModel(item);

                                if (deviceResult.Result?.Devices?.Count > 0)
                                {
                                    foreach (var devices in deviceResult.Result.Devices)
                                    {
                                        i++;
                                        PushNotificationDevice pushNotificationDevice =
                                            new PushNotificationDevice
                                            {
                                                PushNotificationId = pushNotification.Id,
                                                JobName = model.RemindMe ? "Job" + currentTime.ToString("ddMMyyyyHHmmssfff") + i : null,
                                                JobGroup = model.RemindMe ? "Group" + currentTime.ToString("ddMMyyyyHHmmssfff") + i : null,
                                                DeviceId = devices.Id
                                            };
                                        pushNotification.PushNotificationDevices.Add(pushNotificationDevice);
                                    }
                                }
                            }
                        }
                    }

                    _pushNotificationService.UpdatePushNotification(pushNotification);

                    _notificationService.SuccessNotification("Push Notification has been updated successfully.");

                    //selected tab
                    SaveSelectedTabName();

                    //Others
                    if (model.RemindMe)
                    {
                        foreach (var item in pushNotification.PushNotificationDevices)
                        {
                            if (item.Device != null && !string.IsNullOrEmpty(item.Device.TokenId))
                            {
                                //why not use IScheduledTask? this is repetitive = performance.
                                Scheduler.Start(
                                    item.JobName,
                                    item.JobGroup,
                                    model.Title,
                                    model.Description,
                                    item.Device.TokenId,
                                    model.StartTime.Value,
                                    model.EndTime.Value.Add(new TimeSpan(23, 59, 59)),
                                    model.SelectedRepeat,
                                    _iconfiguration["APIKey"].ToString());
                            }
                        }
                    }

                    if (!continueEditing)
                        return RedirectToAction("Index");

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
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
                // Validate that the job doesn't already exists
                if (pushNotification.RemindMe)
                {
                    if (pushNotification.PushNotificationDevices != null)
                    {
                        foreach (var item in pushNotification.PushNotificationDevices)
                        {
                            if (await scheduler.CheckExists(new JobKey(item.JobName, item.JobGroup)))
                                await scheduler.DeleteJob(new JobKey(item.JobName, item.JobGroup));
                        }
                    }
                }

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

        private List<StockTakeHeader> GetStockTakeStore(int? stockTakeNo)
        {
            string conString = DataSettingsManager.LoadSettings().DataConnectionString;

            var model = new PushNotificationModel();
            List<StockTakeHeader> stockTakeList = new List<StockTakeHeader>();
            List<StoreList> storeList = new List<StoreList>();

            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();
                storeList = new List<StoreList>();

                string sSQL = "SELECT STCM.P_StockTakeNo, -99 AS P_BranchNo, 'ALL' AS P_Name FROM [dbo].[StockTakeControlMaster] STCM";
                sSQL += " INNER JOIN [dbo].[Store] S ON STCM.P_BranchNo = S.P_BranchNo";
                sSQL += " WHERE STCM.P_EndDate >= GETDATE() AND STCM.P_BranchNo != 1";

                if (stockTakeNo.HasValue)
                {
                    sSQL += "AND STCM.P_StockTakeNo = '" + stockTakeNo + "'";
                }

                sSQL += " UNION";
                sSQL += " SELECT STCM.P_StockTakeNo, STCOM.P_BranchNo, S.P_Name FROM [dbo].[StockTakeControlMaster] STCM";
                sSQL += " RIGHT JOIN [dbo].[StockTakeControlOutletMaster] STCOM ON STCM.P_StockTakeNo = STCOM.P_StockTakeNo AND STCM.P_BranchNo = 1";
                sSQL += " INNER JOIN [dbo].[Store] S ON STCOM.P_BranchNo = S.P_BranchNo "; //--LEFT
                sSQL += " WHERE STCM.P_EndDate >= GETDATE()";

                if (stockTakeNo.HasValue)
                {
                    sSQL += "AND STCM.P_StockTakeNo = '" + stockTakeNo + "'";
                }

                using (SqlCommand command = new SqlCommand(sSQL, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var stNo = reader["P_StockTakeNo"].ToString();
                        var storeNo = reader["P_BranchNo"].ToString();
                        var storeName = reader["P_Name"].ToString();

                        if (stockTakeList.Any() & stockTakeList.Count > 0)
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
        public ActionResult GetStockTakeStores(int? stockTakeNo)
        {
            var stockTakeStores = new List<string>();
            List<StockTakeHeader> stList = GetStockTakeStore(stockTakeNo);
            string storeNames = string.Empty;
            foreach (var parent in stList)
            {
                foreach (var child in parent.Stores)
                {
                    storeNames += child.StoreName == "ALL" ? child.StoreName : child.StoreNo + " - " + child.StoreName;
                    storeNames += ", ";
                }
            }

            return Json(storeNames.Substring(0, storeNames.LastIndexOf(",", StringComparison.OrdinalIgnoreCase)));
        }
    }
}