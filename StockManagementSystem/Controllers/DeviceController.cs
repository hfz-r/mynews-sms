using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Devices;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StockManagementSystem.Controllers
{
    public class DeviceController : BaseController
    {
        private readonly IDeviceService _deviceService;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IDeviceModelFactory _deviceModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        #region Constructor

        public DeviceController(
            IDeviceService deviceService,
            IRepository<Device> deviceRepository,
            IRepository<Store> storeRepository,
            IDeviceModelFactory deviceModelFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILoggerFactory loggerFactory)
        {
            this._deviceService = deviceService;
            this._deviceRepository = deviceRepository;
            this._storeRepository = storeRepository;
            _deviceModelFactory = deviceModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _logger = loggerFactory.CreateLogger<DeviceController>();
        }

        public ILogger Logger { get; }

        #endregion

        #region Manage Device
        
        public async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            var model = await _deviceModelFactory.PrepareDeviceSearchModel(new DeviceSearchModel());

            return View(model);
        }

        public async Task<IActionResult> GetStore()
        {
            var model = await _deviceModelFactory.PrepareDeviceSearchModel(new DeviceSearchModel());

            return View(model);
        }

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

            try
            {
                Device device = new Device();
                device.ModelNo = model.ModelNo;
                device.StoreId = model.SelectedStoreId;

                //Serial No
                if (!string.IsNullOrWhiteSpace(model.SerialNo))
                    await _deviceService.SetSerialNo(device, model.SerialNo);
                else
                    device.SerialNo = model.SerialNo;

                await _deviceService.InsertDevice(device);
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
        public async Task<IActionResult> DeviceList(DeviceSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedKendoGridJson();

            var model = await _deviceModelFactory.PrepareDeviceListModel(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> EditDevice(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
                return RedirectToAction("Index");

            var model = await _deviceModelFactory.PrepareDeviceModel(null, device);
            model.SelectedStoreId = device.StoreId;

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
                    device.ModelNo = model.ModelNo;
                    device.StoreId = model.SelectedStoreId;

                    //Serial No
                    if (!string.IsNullOrWhiteSpace(model.SerialNo))
                        await _deviceService.SetSerialNo(device, model.SerialNo);
                    else
                        device.SerialNo = model.SerialNo;
                    
                    _deviceService.UpdateDevice(device);

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

                _notificationService.SuccessNotification("Device has been deleted successfully.");

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return RedirectToAction("Edit", new { id = device.Id });
            }
        }
        
        #endregion

        #region Device Tracking

        [HttpGet]
        public async Task<IActionResult> DeviceTracking()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDevices))
                return AccessDeniedView();

            var model = await _deviceModelFactory.PrepareDeviceListModel();

            return View(model);
        }

        #endregion

        #region Private Method

        private DeviceViewModel GetAllDevice()
        {
            DeviceViewModel model = new DeviceViewModel
            {
                Device = _deviceRepository.Table.ToList()
            };

            return model;
        }

        #endregion  
    }
}