using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Factories;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Locations;
using StockManagementSystem.Models.OrderLimits;
using StockManagementSystem.Services.Locations;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.OrderLimits;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Controllers
{
    public class SettingController : BaseController
    {
        private readonly IOrderLimitService _orderLimitService;
        private readonly IStoreService _storeService;
        private readonly ILocationService _locationService;
        private readonly IRepository<Approval> _approvalRepository;
        private readonly IRepository<OrderLimit> _orderLimitRepository;
        private readonly IRepository<OrderLimitStore> _orderLimitStoreRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<ShelfLocation> _shelfLocationRepository;
        private readonly IRepository<ShelfLocationFormat> _shelfLocationFormatRepository;
        private readonly IOrderLimitModelFactory _orderLimitModelFactory;
        private readonly ILocationModelFactory _locationModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        #region Constructor

        public SettingController(
            IOrderLimitService orderLimitService,
            IStoreService storeService,
            ILocationService locationService,
            IRepository<Approval> approvalRepository,
            IRepository<OrderLimit> orderLimitRepository,
            IRepository<OrderLimitStore> orderLimitStoreRepository,
            IRepository<Store> storeRepository,
            IRepository<Item> itemRepository,
            IRepository<ShelfLocation> shelfLocationRepository,
            IRepository<ShelfLocationFormat> shelfLocationFormatRepository,
            IOrderLimitModelFactory orderLimitModelFactory,
            ILocationModelFactory locationModelFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILoggerFactory loggerFactory)
        {
            this._orderLimitService = orderLimitService;
            this._storeService = storeService;
            this._locationService = locationService;
            this._approvalRepository = approvalRepository;
            this._orderLimitRepository = orderLimitRepository;
            this._orderLimitStoreRepository = orderLimitStoreRepository;
            this._storeRepository = storeRepository;
            this._itemRepository = itemRepository;
            this._shelfLocationRepository = shelfLocationRepository;
            this._shelfLocationFormatRepository = shelfLocationFormatRepository;
            this._orderLimitModelFactory = orderLimitModelFactory;
            this._locationModelFactory = locationModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _logger = loggerFactory.CreateLogger<SettingController>();
        }

        public ILogger Logger { get; }

        #endregion

        #region Stock Order

        public async Task<IActionResult> Order()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var model = await _orderLimitModelFactory.PrepareOrderLimitSearchModel(new OrderLimitSearchModel());

            return View(model);
        }

        public async Task<IActionResult> GetStore()
        {
            var model = await _orderLimitModelFactory.PrepareOrderLimitSearchModel(new OrderLimitSearchModel());

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddOrderLimit(OrderLimitModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();
            
            if (model.SelectedStoreIds.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Store is required to register a device");
                _notificationService.ErrorNotification("Store is required to register a device");
                return new NullJsonResult();
            }

            try
            {
                OrderLimit orderLimit = new OrderLimit
                {
                    Percentage = model.Percentage,
                    OrderLimitStores = new List<OrderLimitStore>()
                };

                //Add store
                foreach (var store in model.SelectedStoreIds)
                {
                    OrderLimitStore orderLimitStore = new OrderLimitStore
                    {
                        OrderLimitId = orderLimit.Id,
                        StoreId = store
                    };

                    orderLimit.OrderLimitStores.Add(orderLimitStore);
                }

                await _orderLimitService.InsertOrderLimit(orderLimit);

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
        public async Task<IActionResult> OrderLimitList(OrderLimitSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedKendoGridJson();

            var model = await _orderLimitModelFactory.PrepareOrderLimitListModel(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> EditOrder(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var orderLimit = await _orderLimitService.GetOrderLimitByIdAsync(id);
            if (orderLimit == null)
                return RedirectToAction("Order");

            var model = await _orderLimitModelFactory.PrepareOrderLimitModel(null, orderLimit);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> EditOrder(OrderLimitModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var orderLimit = await _orderLimitService.GetOrderLimitByIdAsync(model.Id);
            if (orderLimit == null)
                return RedirectToAction("Order");

            //validate stores
            var allStores = await _storeService.GetStoresAsync();
            var newStores = new List<Store>();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                    newStores.Add(store);
            }

            if(model.SelectedStoreIds.Count == 0)
            {
                _notificationService.ErrorNotification("Store is required");
                model = await _orderLimitModelFactory.PrepareOrderLimitModel(model, orderLimit);
                model.SelectedStoreIds = new List<int>();

                return View(model);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    orderLimit.Percentage = model.Percentage;

                    //stores

                    List<OrderLimitStore> orderLimitStoreList = new List<OrderLimitStore>();

                    foreach (var store in allStores)
                    {
                        if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                        {
                            //new store
                            if (orderLimit.OrderLimitStores.Count(mapping => mapping.StoreId == store.P_BranchNo) == 0)
                            {
                                OrderLimitStore orderLimitStore = new OrderLimitStore
                                {
                                    OrderLimitId = orderLimit.Id,
                                    StoreId = store.P_BranchNo
                                };


                                orderLimit.OrderLimitStores.Add(orderLimitStore);
                            }
                        }
                        else
                        {
                            //remove store
                            if (orderLimit.OrderLimitStores.Count(mapping => mapping.StoreId == store.P_BranchNo) > 0)
                                _orderLimitService.DeleteOrderLimitStore(model.Id, store);
                        }
                    }

                    _orderLimitService.UpdateOrderLimit(orderLimit);

                    _notificationService.SuccessNotification("Order limit has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("Order");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("EditOrder", new { id = orderLimit.Id });
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }
            }

            model = await _orderLimitModelFactory.PrepareOrderLimitModel(model, orderLimit);

            return View(model);
        }
        
        public async Task<IActionResult> DeleteOrderLimit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var orderLimit = await _orderLimitService.GetOrderLimitByIdAsync(id);
            if (orderLimit == null)
                return RedirectToAction("Order");

            try
            {
                _orderLimitService.DeleteOrderLimit(orderLimit);

                _notificationService.SuccessNotification("Order limit has been deleted successfully.");

                return RedirectToAction("Order");
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return RedirectToAction("EditOrder", new { id = orderLimit.Id });
            }
        }
        
        #endregion

        #region Approval

        [HttpGet]
        public IActionResult Approval()
        {
            return View("Approval");
        }

        #endregion

        #region Location
        
        public async Task<IActionResult> Location()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
                return AccessDeniedView();
            
            var model = await _locationModelFactory.PrepareShelfLocationFormatSearchModel(new LocationSearchModel());
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ListLocation(LocationSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
                return AccessDeniedKendoGridJson();

            var model = await _locationModelFactory.PrepareShelfLocationFormatListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            try
            {
                ShelfLocationFormat shelfLocationFormat = new ShelfLocationFormat();
                shelfLocationFormat.Prefix = model.Prefix;
                shelfLocationFormat.Name = model.Name;

                shelfLocationFormat = model.ToEntity(shelfLocationFormat);

                await _locationService.InsertShelfLocationFormat(shelfLocationFormat);

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
        public async Task<IActionResult> UpdateLocation(LocationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            var shelfLocationFormat = await _locationService.GetShelfLocationFormatByIdAsync(model.Id);
            shelfLocationFormat.Prefix = model.Prefix;
            shelfLocationFormat.Name = model.Name;

            shelfLocationFormat = model.ToEntity(shelfLocationFormat);

            _locationService.UpdateShelfLocationFormat(shelfLocationFormat);
          
            return new NullJsonResult();
        }

        public async Task<IActionResult> DeleteLocation(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
                return AccessDeniedView();

            var shelfLocationFormat = await _locationService.GetShelfLocationFormatByIdAsync(id) ?? throw new ArgumentException("No prefix name found with the specified id", nameof(id));
            _locationService.DeleteShelfLocationFormat(shelfLocationFormat);

            _notificationService.SuccessNotification("Prefix name has been deleted successfully.");

            return new NullJsonResult();
        }

        #endregion

        private void AddErrors(string result)
        {
            ModelState.AddModelError(string.Empty, result);
        }
    }
}