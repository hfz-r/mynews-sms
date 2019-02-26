using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Devices;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Setting;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Tenants;

namespace StockManagementSystem.Controllers
{
    public class DeviceController : BaseController
    {
        private readonly IDeviceService _deviceService;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly ITenantService _tenantService;
        private readonly ITenantMappingService _tenantMappingService;
        private readonly IDeviceModelFactory _deviceModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        #region Constructor

        public DeviceController(
            IDeviceService deviceService,
            IRepository<Device> deviceRepository,
            IRepository<Store> storeRepository,
            ITenantService tenantService,
            ITenantMappingService tenantMappingService,
            IDeviceModelFactory deviceModelFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILoggerFactory loggerFactory)
        {
            _deviceService = deviceService;
            _deviceRepository = deviceRepository;
            _storeRepository = storeRepository;
            _tenantService = tenantService;
            _tenantMappingService = tenantMappingService;
            _deviceModelFactory = deviceModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _logger = loggerFactory.CreateLogger<DeviceController>();
        }

        public ILogger Logger { get; }

        #endregion

        #region Utilities

        /// <summary>
        /// Multi-tenant update/save entity
        /// </summary>
        protected async Task SaveTenantMappings(Device device, DeviceModel model)
        {
            device.LimitedToTenants = model.SelectedTenantIds.Any();

            var existingTenantMappings = await _tenantMappingService.GetTenantMappings(device);
            var tenants = await _tenantService.GetTenantsAsync();
            foreach (var tenant in tenants)
            {
                if (model.SelectedTenantIds.Contains(tenant.Id))
                {
                    //new tenant
                    if (existingTenantMappings.Count(tm => tm.TenantId == tenant.Id) == 0)
                        await _tenantMappingService.InsertTenantMapping(device, tenant.Id);
                }
                else
                {
                    //remove tenant
                    var tenantMappingToDelete = existingTenantMappings.FirstOrDefault(tm => tm.TenantId == tenant.Id);
                    if (tenantMappingToDelete != null)
                        await _tenantMappingService.DeleteTenantMapping(tenantMappingToDelete);
                }
            }
        }

        #endregion

        #region Manage Device

        public async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            var model = await _deviceModelFactory.PrepareDeviceSearchModel(new DeviceSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeviceList(DeviceSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedKendoGridJson();

            var model = await _deviceModelFactory.PrepareDeviceListModel(searchModel);

            return Json(model);
        }

        #region Create / Edit / Delete

        [HttpPost]
        public async Task<IActionResult> AddDevice(DeviceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            //validate stores
            if (model.SelectedStoreId == 0)
            {
                ModelState.AddModelError(string.Empty, "Store is required to register a device");
                _notificationService.ErrorNotification("Store is required to register a device");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var device = model.ToEntity<Device>();
                    device.CreatedOnUtc = DateTime.UtcNow;
                    device.ModifiedOnUtc = DateTime.UtcNow;
                    device.StoreId = model.SelectedStoreId;
                    device.Status = "0";

                    //Serial No
                    if (!string.IsNullOrWhiteSpace(model.SerialNo))
                        await _deviceService.SetSerialNo(device, model.SerialNo);
                    else
                        device.SerialNo = model.SerialNo;

                    await _deviceService.InsertDevice(device);

                    //tenants - currently not wired; on test
                    await SaveTenantMappings(device, model);

                    return new NullJsonResult();
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    _notificationService.ErrorNotification(e.Message);

                    return Json(e.Message);
                }
            }

            return new NullJsonResult();
        }

        public async Task<IActionResult> EditDevice(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
                return RedirectToAction("Index");

            var model = await _deviceModelFactory.PrepareDeviceModel(null, device);
            //model.SelectedStoreId = device.StoreId;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> EditDevice(DeviceModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            var device = await _deviceService.GetDeviceByIdAsync(model.Id);
            if (device == null)
                return RedirectToAction("Index");

            //validate stores
            if (model.SelectedStoreId == 0)
            {
                ModelState.AddModelError(string.Empty, "Store is required to register a device");
                _notificationService.ErrorNotification("Store is required to register a device");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    device = model.ToEntity(device);
                    device.ModifiedOnUtc = DateTime.UtcNow;
                    device.ModelNo = model.ModelNo;
                    device.StoreId = model.SelectedStoreId;

                    //Serial No
                    if (!string.IsNullOrWhiteSpace(model.SerialNo))
                        await _deviceService.SetSerialNo(device, model.SerialNo);
                    else
                        device.SerialNo = model.SerialNo;

                    _deviceService.UpdateDevice(device);

                    //tenants - currently not wired; on test
                    await SaveTenantMappings(device, model);

                    _notificationService.SuccessNotification("Device has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("Index");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("EditDevice", new { id = device.Id });
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }
            }

            model = await _deviceModelFactory.PrepareDeviceModel(model, device);

            return View(model);
        }

        public async Task<IActionResult> DeleteDevice(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
                return RedirectToAction("Index");

            try
            {
                //remove
                _deviceService.DeleteDevice(device);

                //TODO: delete on parent level - storemapping

                _notificationService.SuccessNotification("Device has been deleted successfully.");

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return RedirectToAction("EditDevice", new { id = device.Id });
            }
        }

        #endregion

        #endregion

        #region Device Tracking

        public async Task<IActionResult> DeviceTracking()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            var model = await _deviceModelFactory.PrepareDeviceTrackingContainerModel(new DeviceTrackingContainerModel());

            model.DeviceMap = await _deviceModelFactory.PrepareDeviceListModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MapList(MapDeviceSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedKendoGridJson();

            var model = await _deviceModelFactory.PrepareMapDeviceListingModel(searchModel);
            return Json(model);
        }
        #endregion
    }
}