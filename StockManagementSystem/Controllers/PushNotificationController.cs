using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.PushNotifications;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.PushNotifications;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Controllers
{
    public class PushNotificationController : BaseController
    {
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IStoreService _storeService;
        private readonly IRepository<PushNotification> _pushNotificationRepository;
        private readonly IRepository<PushNotificationStore> _pushNotificationStoreRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IPushNotificationModelFactory _pushNotificationModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        #region Constructor

        public PushNotificationController(
            IPushNotificationService pushNotificationService,
            IStoreService storeService,
            IRepository<PushNotification> pushNotificationRepository,
            IRepository<PushNotificationStore> pushNotificationStoreRepository,
            IRepository<Store> storeRepository,
            IPushNotificationModelFactory pushNotificationModelFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILoggerFactory loggerFactory)
        {
            this._pushNotificationService = pushNotificationService;
            this._storeService = storeService;
            this._pushNotificationRepository = pushNotificationRepository;
            this._pushNotificationStoreRepository = pushNotificationStoreRepository;
            this._storeRepository = storeRepository;
            this._pushNotificationModelFactory = pushNotificationModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
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

        public async Task<IActionResult> GetStore()
        {
            var model = await _pushNotificationModelFactory.PreparePushNotificationSearchModel(new PushNotificationSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddNotification(PushNotificationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePushNotification))
                return AccessDeniedView();

            if (model.SelectedStoreIds.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Store is required to create push notification");
                _notificationService.ErrorNotification("Store is required to create push notification");
                return new NullJsonResult();
            }

            if (model.SelectedNotificationCategoryIds.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Notification Category is required to create push notification");
                _notificationService.ErrorNotification("Notification Category is required to create push notification");
                return new NullJsonResult();
            }

            try
            {
                PushNotification pushNotification = new PushNotification();
                pushNotification.Title = model.Title;
                pushNotification.Desc = model.Description;
                pushNotification.StockTakeNo = string.IsNullOrEmpty(model.StockTakeNo) ? string.Empty : model.StockTakeNo;
                pushNotification.NotificationCategoryId = model.SelectedNotificationCategoryIds.FirstOrDefault();
                pushNotification.PushNotificationStores = new List<PushNotificationStore>();

                //Add store
                foreach (var store in model.SelectedStoreIds)
                {
                    PushNotificationStore pushNotificationStore = new PushNotificationStore();
                    pushNotificationStore.PushNotificationId = pushNotification.Id;
                    pushNotificationStore.StoreId = store;

                    pushNotification.PushNotificationStores.Add(pushNotificationStore);
                }

                await _pushNotificationService.InsertPushNotification(pushNotification);

                return new NullJsonResult();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                _notificationService.ErrorNotification(e.Message);

                return Json(e.Message);
            }
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
            var allStores = await _storeService.GetStoresAsync();
            var newStores = new List<Store>();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                    newStores.Add(store);
            }

            if (model.SelectedStoreIds.Count == 0)
            {
                _notificationService.ErrorNotification("Store is required");
                model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                model.SelectedStoreIds = new List<int>();

                return View(model);
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
                if(model.SelectedNotificationCategoryIds.FirstOrDefault() == 1)
                {
                    if (string.IsNullOrEmpty(model.StockTakeNo))
                    {
                        _notificationService.ErrorNotification("Stock Take # is required");
                        model = await _pushNotificationModelFactory.PreparePushNotificationModel(model, pushNotification);
                        model.StockTakeNo = string.Empty;

                        return View(model);
                    }
                }
                else
                {
                    model.StockTakeNo = string.Empty;
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
    }
}