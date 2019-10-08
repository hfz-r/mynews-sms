using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.LicenseManager.Domain;
using StockManagementSystem.LicenseManager.Factories;
using StockManagementSystem.LicenseManager.Models;
using StockManagementSystem.LicenseManager.Services;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Media;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;

namespace StockManagementSystem.LicenseManager.Controllers
{
    public class LicenseController : BasePluginController
    {
        private readonly IUserActivityService _userActivityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IDeviceService _deviceService;
        private readonly ILicenseService _licenseService;
        private readonly IDownloadService _downloadService;
        private readonly ILicenseModelFactory _licenseModelFactory;

        public LicenseController(
            IUserActivityService userActivityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IDeviceService deviceService,
            ILicenseService licenseService,
            IDownloadService downloadService,
            ILicenseModelFactory licenseModelFactory)
        {
            _userActivityService = userActivityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _deviceService = deviceService;
            _licenseService = licenseService;
            _downloadService = downloadService;
            _licenseModelFactory = licenseModelFactory;
        }

        #region Utilities

        protected async Task<string> ValidateLicense(LicenseModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var license = await _licenseService.GetLicenseById(model.Id) != null;
            return license ? $"The license with (ID = {model.Id}) existed." : string.Empty;
        }

        protected async Task<bool> DeleteLicense(int id)
        {
            var license = await _licenseService.GetLicenseById(id);
            if (license == null)
                return false;

            await _licenseService.DeleteLicense(license);

            return true;
        }

        #endregion

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = await _licenseModelFactory.PrepareLicenseConfigurationModel(new ConfigurationModel());

            return View(ViewNames.Configuration, model);
        }

        [HttpPost]
        public async Task<IActionResult> List(ConfigurationModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedKendoGridJson();

            var model = await _licenseModelFactory.PrepareLicenseListModel(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = await _licenseModelFactory.PrepareLicenseModel(new LicenseModel(), null);

            return View(ViewNames.Create, model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Create(LicenseModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var licenseError = await ValidateLicense(model);
            if (!string.IsNullOrEmpty(licenseError))
            {
                ModelState.AddModelError(string.Empty, licenseError);
                _notificationService.ErrorNotification(licenseError);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var license = model.ToEntity<License>();

                    await _licenseService.InsertLicense(license);
                    await _userActivityService.InsertActivityAsync("AddNewLicense", $"Added a new license (ID = {license.Id})");

                    _notificationService.SuccessNotification("The new license has been added successfully.");

                    if (!continueEditing)
                        return RedirectToAction("Configure");

                    return RedirectToAction("Edit", new { id = license.Id });

                }
                catch (Exception ex)
                {
                    _notificationService.ErrorNotification(ex.Message);
                }
            }

            model = await _licenseModelFactory.PrepareLicenseModel(model, null, true);

            return View(ViewNames.Create, model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var license = await _licenseService.GetLicenseById(id);
            if (license == null)
                return RedirectToAction("Configure");

            var model = await _licenseModelFactory.PrepareLicenseModel(null, license);

            return View(ViewNames.Edit, model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(LicenseModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var license = await _licenseService.GetLicenseById(model.Id);
            if (license == null)
                return RedirectToAction("Configure");

            if (ModelState.IsValid)
            {
                try
                {
                    license = model.ToEntity(license);
                  
                    await _licenseService.UpdateLicense(license);
                    await _userActivityService.InsertActivityAsync("EditLicense", $"Edited license (ID = {model.Id})");

                    _notificationService.SuccessNotification("License has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("Configure");


                    return RedirectToAction("Edit", new { id = model.Id });
                }
                catch (Exception ex)
                {
                    _notificationService.ErrorNotification(ex.Message);
                }
            }

            model = await _licenseModelFactory.PrepareLicenseModel(model, license, true);

            return View(ViewNames.Edit, model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteInline(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var result = await DeleteLicense(id);
            if (!result)
                throw new ArgumentException("No license found with the specified id", nameof(id));

            await _userActivityService.InsertActivityAsync("DeleteLicense", $"Deleted a license (ID = {id})");

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var result = await DeleteLicense(id);
            if (!result)
                return RedirectToAction("Configure");

            await _userActivityService.InsertActivityAsync("DeleteLicense", $"Deleted a license (ID = {id})");

            _notificationService.SuccessNotification("License has been deleted successfully.");

            return RedirectToAction("Configure");
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("generate-license")]
        public async Task<IActionResult> GenerateLicense(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                await _licenseService.GenerateLicense(id);

                await _userActivityService.InsertActivityAsync("GenerateLicense", "License generated");

                _notificationService.SuccessNotification("The license have been generated successfully.");
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification(ex.Message);
            }

            return RedirectToAction("Edit");
        }

        public async Task<IActionResult> DownloadLicense(int downloadId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var download = await _downloadService.GetDownloadById(downloadId);
            if (download == null)
                return Content("No download record found with the specified id");

            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType)
                ? download.ContentType
                : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType)
            {
                FileDownloadName = fileName + download.Extension
            };
        }

        #region DeviceLicense

        [HttpPost]
        public async Task<IActionResult> DeviceLicenseList(DeviceLicenseSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedKendoGridJson();

            var license = await _licenseService.GetLicenseById(searchModel.LicenseId) ??
                          throw new ArgumentException("No license found with the specified id");

            var model = await _licenseModelFactory.PrepareDeviceLicenseListModel(searchModel, license);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeviceLicenseDelete(int licenseId, string serialNo)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var license = await _licenseService.GetLicenseById(licenseId) ??
                          throw new ArgumentException("No license found with the specified id", nameof(licenseId));

            var device = await _deviceService.GetDeviceBySerialNoAsync(serialNo) ??
                         throw new ArgumentException("No device found with the specified serial no", nameof(serialNo));

            if (device.DeviceLicenses.Count(mapping => mapping.LicenseId == license.Id) > 0)
                device.DeviceLicenses.Remove(device.DeviceLicenses.FirstOrDefault(m => m.LicenseId == license.Id));

            await _deviceService.UpdateDeviceAsync(device);

            return new NullJsonResult();
        }

        public async Task<IActionResult> AssignDevicePopup(int licenseId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = await _licenseModelFactory.PrepareAssignDeviceSearchModel(new AssignDeviceSearchModel());

            return View(ViewNames.AssignDevicePopup, model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignDevicePopupList(AssignDeviceSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedKendoGridJson();

            var model = await _licenseModelFactory.PrepareAssignDeviceListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> AssignDevicePopup(AssignDeviceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var license = await _licenseService.GetLicenseById(model.LicenseId) ??
                          throw new ArgumentException("No license found with the specified id");

            foreach (var id in model.SelectedDeviceIds)
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                if (device == null)
                    continue;

                if (device.DeviceLicenses.Count(mapping => mapping.LicenseId == license.Id) == 0)
                    device.DeviceLicenses.Add(new DeviceLicense
                    {
                        LicenseId = license.Id,
                        Device = device
                    });

                await _deviceService.UpdateDeviceAsync(device);
            }

            ViewBag.RefreshPage = true;

            return View(ViewNames.AssignDevicePopup, new AssignDeviceSearchModel());
        }

        #endregion
    }
}