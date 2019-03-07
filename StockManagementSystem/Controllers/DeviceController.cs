using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Devices;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Setting;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;

namespace StockManagementSystem.Controllers
{
    public class DeviceController : BaseController
    {
        private readonly IDeviceService _deviceService;
        private readonly ITenantService _tenantService;
        private readonly ITenantMappingService _tenantMappingService;
        private readonly IDeviceModelFactory _deviceModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IUserActivityService _userActivityService;

        public DeviceController(
            IDeviceService deviceService,
            ITenantService tenantService,
            ITenantMappingService tenantMappingService,
            IDeviceModelFactory deviceModelFactory,
            IPermissionService permissionService,
            IUserActivityService userActivityService)
        {
            _deviceService = deviceService;
            _tenantService = tenantService;
            _tenantMappingService = tenantMappingService;
            _deviceModelFactory = deviceModelFactory;
            _permissionService = permissionService;
            _userActivityService = userActivityService;
        }

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

        [HttpPost]
        public async Task<IActionResult> AddDevice(DeviceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            var device = model.ToEntity<Device>();
            device.CreatedOnUtc = DateTime.UtcNow;
            device.ModifiedOnUtc = DateTime.UtcNow;
            device.StoreId = model.SelectedStoreId;
            device.Status = "0";

            if (!string.IsNullOrWhiteSpace(model.SerialNo))
                await _deviceService.SetSerialNo(device, model.SerialNo);
            else
                device.SerialNo = model.SerialNo;

            await _deviceService.InsertDevice(device);

            //tenant mapping
            await SaveTenantMappings(device, model);

            await _userActivityService.InsertActivityAsync("AddNewDevice", $"Added a new device (Id: '{device.Id}')", device);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> EditDevice(DeviceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            var device = await _deviceService.GetDeviceByIdAsync(model.Id) ??
                         throw new ArgumentException("No device found with the specified id", nameof(model.Id));

            device = model.ToEntity(device);
            device.ModifiedOnUtc = DateTime.UtcNow;
            device.ModelNo = model.ModelNo;
            device.StoreId = model.SelectedStoreId;

            if (!string.IsNullOrWhiteSpace(model.SerialNo))
                await _deviceService.SetSerialNo(device, model.SerialNo);
            else
                device.SerialNo = model.SerialNo;

            _deviceService.UpdateDevice(device);

            //tenant mapping
            await SaveTenantMappings(device, model);

            await _userActivityService.InsertActivityAsync("EditDevice", $"Edited a device (Id: '{device.Id}')", device);

            return new NullJsonResult();
        }

        public async Task<IActionResult> DeleteDevice(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            var device = await _deviceService.GetDeviceByIdAsync(id) ??
                         throw new ArgumentException("No device found with the specified id", nameof(id));

            _deviceService.DeleteDevice(device);

            await _userActivityService.InsertActivityAsync("DeleteDevice", $"Deleted a device (Id: '{id}')");

            return new NullJsonResult();
        }

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