using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Media;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Factories;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Locations;
using StockManagementSystem.Models.OrderLimits;
using StockManagementSystem.Models.Replenishments;
using StockManagementSystem.Models.Setting;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Locations;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Media;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.OrderLimits;
using StockManagementSystem.Services.Replenishments;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Settings;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Controllers
{
    public class SettingController : BaseController
    {
        private readonly IOrderLimitService _orderLimitService;
        private readonly IReplenishmentService _replenishmentService;
        private readonly ILocationService _locationService;
        private readonly IFormatSettingService _formatSettingService;
        private readonly IOrderLimitModelFactory _orderLimitModelFactory;
        private readonly IReplenishmentModelFactory _replenishmentModelFactory;
        private readonly ILocationModelFactory _locationModelFactory;
        private readonly IFormatSettingModelFactory _formatSettingModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IWorkContext _workContext;
        private readonly ITenantContext _tenantContext;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IEncryptionService _encryptionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreService _storeService;
        private readonly ITenantService _tenantService;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly IRepository<ShelfLocation> _shelfLocationRepository;

        public SettingController(
            IOrderLimitService orderLimitService, 
            IReplenishmentService replenishmentService,
            ILocationService locationService, 
            IFormatSettingService formatSettingService,
            IOrderLimitModelFactory orderLimitModelFactory, 
            IReplenishmentModelFactory replenishmentModelFactory,
            ILocationModelFactory locationModelFactory, 
            IFormatSettingModelFactory formatSettingModelFactory,
            IPermissionService permissionService, 
            INotificationService notificationService, 
            IWorkContext workContext,
            ITenantContext tenantContext, 
            IConfiguration configuration, 
            IDateTimeHelper dateTimeHelper,
            IUserService userService, 
            IUserActivityService userActivityService, 
            IEncryptionService encryptionService,
            IGenericAttributeService genericAttributeService, 
            IStoreService storeService, 
            ITenantService tenantService,
            ISettingService settingService, 
            IPictureService pictureService, 
            ISettingModelFactory settingModelFactory,
            IRepository<ShelfLocation> shelfLocationRepository)
        {
            _orderLimitService = orderLimitService;
            _replenishmentService = replenishmentService;
            _locationService = locationService;
            _formatSettingService = formatSettingService;
            _orderLimitModelFactory = orderLimitModelFactory;
            _replenishmentModelFactory = replenishmentModelFactory;
            _locationModelFactory = locationModelFactory;
            _formatSettingModelFactory = formatSettingModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _workContext = workContext;
            _tenantContext = tenantContext;
            _configuration = configuration;
            _dateTimeHelper = dateTimeHelper;
            _userService = userService;
            _userActivityService = userActivityService;
            _encryptionService = encryptionService;
            _genericAttributeService = genericAttributeService;
            _storeService = storeService;
            _tenantService = tenantService;
            _settingService = settingService;
            _pictureService = pictureService;
            _settingModelFactory = settingModelFactory;
            _shelfLocationRepository = shelfLocationRepository;
        }

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
        [FormValueRequired("save")]
        public async Task<IActionResult> AddOrderLimit(OrderLimitModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            OrderBranchMaster orderLimit = new OrderBranchMaster();

            if (ModelState.IsValid)
            {
                try
                {
                    //var isExist = await _orderLimitService.IsStoreExistAsync(model.SelectedStoreIds);
                    //if (isExist)
                    //{
                    //    ModelState.AddModelError(string.Empty, "Store has existed in current order limit.");
                    //    _notificationService.ErrorNotification("Store has existed in current order limit.");
                    //    return new NullJsonResult();
                    //}

                    orderLimit = new OrderBranchMaster
                    {
                        P_DeliveryPerWeek = model.DeliveryPerWeek,
                        P_Safety = model.Safety,
                        P_InventoryCycle = model.InventoryCycle,
                        P_OrderRatio = model.OrderRatio,
                        P_FaceQty = model.FaceQty,
                        P_MinDays = model.MinDays,
                        P_MaxDays = model.MaxDays,
                        //P_BranchNo = model.SelectedStoreIds,
                        Status = 1
                    };

                    await _orderLimitService.InsertOrderLimit(orderLimit);

                    return RedirectToAction("Order");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    _notificationService.ErrorNotification(e.Message);

                    return Json(e.Message);
                }
            }
            else { return View(model); }
        }
        
        public async Task<IActionResult> AddOrderLimit()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var model = await _orderLimitModelFactory.PrepareOrderLimitModel(null, null);

            return View(model);
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

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> EditOrder(OrderLimitModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var orderLimit = await _orderLimitService.GetOrderLimitByIdAsync(model.Id);
            if (orderLimit == null)
                return RedirectToAction("Order");

            //validate stores
            var allStores = await _storeService.GetStores();
            var newStores = new List<Store>();
            foreach (var store in allStores)
                if (model.SelectedStoreIds == store.P_BranchNo)
                    newStores.Add(store);

            if (ModelState.IsValid)
                try
                {
                    orderLimit.P_DeliveryPerWeek = model.DeliveryPerWeek;
                    orderLimit.P_Safety = model.Safety;
                    orderLimit.P_InventoryCycle = model.InventoryCycle;
                    orderLimit.P_OrderRatio = model.OrderRatio;
                    orderLimit.P_MaxDays = model.MaxDays;
                    orderLimit.P_MinDays = model.MinDays;
                    orderLimit.P_FaceQty = model.FaceQty;
                    // orderLimit.P_BranchNo = model.SelectedStoreIds;

                    _orderLimitService.UpdateOrderLimit(orderLimit);

                    _notificationService.SuccessNotification("Order limit has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("Order");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("EditOrder", new {id = orderLimit.Id});
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
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
                return RedirectToAction("EditOrder", new {id = orderLimit.Id});
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

        #region Tenant scope configuration

        public async Task<IActionResult> ChangeTenantScopeConfiguration(int tenantId, string returnUrl = "")
        {
            var tenant = _tenantService.GetTenantById(tenantId);
            if (tenant != null || tenantId == 0)
                await _genericAttributeService.SaveAttributeAsync(_workContext.CurrentUser,
                    UserDefaults.TenantScopeConfigurationAttribute, tenantId);

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home");

            return Redirect(returnUrl);
        }

        #endregion

        #region Settings

        public async Task<IActionResult> GeneralCommon()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = await _settingModelFactory.PrepareGeneralCommonSettingsModel();

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> GeneralCommon(GeneralCommonSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var tenantScope = _tenantContext.ActiveTenantScopeConfiguration;

            //common settings
            var commonSettings = _settingService.LoadSetting<CommonSettings>(tenantScope);
            commonSettings.LogoPictureId = model.CommonSettings.LogoPictureId;
            commonSettings.UseResponseCompression = model.CommonSettings.UseResponseCompression;

            await _settingService.SaveSettingOverridablePerTenant(commonSettings, x => x.LogoPictureId, model.CommonSettings.LogoPictureId_OverrideForTenant, tenantScope, false);
            await _settingService.SaveSettingOverridablePerTenant(commonSettings, x => x.UseResponseCompression, model.CommonSettings.UseResponseCompression_OverrideForTenant, tenantScope, false);

            _settingService.ClearCache();

            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(tenantScope);
            if (securitySettings.AllowedIpAddresses == null)
                securitySettings.AllowedIpAddresses = new List<string>();
            securitySettings.AllowedIpAddresses.Clear();

            if (!string.IsNullOrEmpty(model.SecuritySettings.AllowedIpAddresses))
                foreach (var s in model.SecuritySettings.AllowedIpAddresses.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    if (!string.IsNullOrWhiteSpace(s))
                        securitySettings.AllowedIpAddresses.Add(s.Trim());

            securitySettings.ForceSslForAllPages = model.SecuritySettings.ForceSslForAllPages;
            securitySettings.EnableXsrfProtection = model.SecuritySettings.EnableXsrfProtection;

            _settingService.SaveSetting(securitySettings);

            await _userActivityService.InsertActivityAsync("EditSettings", "Edited settings");

            _notificationService.SuccessNotification("The settings have been updated successfully.");

            return RedirectToAction("GeneralCommon");
        }

        public async Task<IActionResult> UserAdmin()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = await _settingModelFactory.PrepareUserAdminSettingsModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserAdmin(UserAdminSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var tenantScope = _tenantContext.ActiveTenantScopeConfiguration;
            var userSettings = _settingService.LoadSetting<UserSettings>(tenantScope);

            var lastUsernameValidationRule = userSettings.UsernameValidationRule;
            var lastUsernameValidationEnabledValue = userSettings.UsernameValidationEnabled;
            var lastUsernameValidationUseRegexValue = userSettings.UsernameValidationUseRegex;

            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(tenantScope);

            userSettings = model.UserSettings.ToSettings(userSettings);

            if (userSettings.UsernameValidationEnabled && userSettings.UsernameValidationUseRegex)
            {
                try
                {
                    //validate regex rule
                    var unused = Regex.IsMatch("test_user_name", userSettings.UsernameValidationRule);
                }
                catch (ArgumentException)
                {
                    //restoring previous settings
                    userSettings.UsernameValidationRule = lastUsernameValidationRule;
                    userSettings.UsernameValidationEnabled = lastUsernameValidationEnabledValue;
                    userSettings.UsernameValidationUseRegex = lastUsernameValidationUseRegexValue;

                    _notificationService.ErrorNotification("The regular expression for username validation is incorrect");
                }
            }

            _settingService.SaveSetting(userSettings);

            dateTimeSettings.DefaultTimeZoneId = model.DateTimeSettings.DefaultTimeZoneId;
            dateTimeSettings.AllowUsersToSetTimeZone = model.DateTimeSettings.AllowUsersToSetTimeZone;
            _settingService.SaveSetting(dateTimeSettings);

            await _userActivityService.InsertActivityAsync("EditSettings", "Edited settings");

            _notificationService.SuccessNotification("The settings have been updated successfully.");

            SaveSelectedTabName();

            return RedirectToAction("UserAdmin");
        }

        public async Task<IActionResult> Media()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = await _settingModelFactory.PrepareMediaSettingsModel();

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> Media(MediaSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var tenantScope = _tenantContext.ActiveTenantScopeConfiguration;
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(tenantScope);
            mediaSettings = model.ToSettings(mediaSettings);

            await _settingService.SaveSettingOverridablePerTenant(mediaSettings, x => x.AvatarPictureSize, model.AvatarPictureSize_OverrideForTenant, tenantScope, false);
            await _settingService.SaveSettingOverridablePerTenant(mediaSettings, x => x.MaximumImageSize, model.MaximumImageSize_OverrideForTenant, tenantScope, false);
            await _settingService.SaveSettingOverridablePerTenant(mediaSettings, x => x.DefaultImageQuality, model.DefaultImageQuality_OverrideForTenant, tenantScope, false);

            _settingService.ClearCache();

            await _userActivityService.InsertActivityAsync("EditSettings", "Edited settings");

            _notificationService.SuccessNotification("The settings have been updated successfully.");

            return RedirectToAction("Media");
        }

        [HttpPost, ActionName("Media")]
        [FormValueRequired("change-picture-storage")]
        public async Task<IActionResult> ChangePictureStorage()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _pictureService.StoreInDb = !_pictureService.StoreInDb;

            await _userActivityService.InsertActivityAsync("EditSettings", "Edited settings");

            _notificationService.SuccessNotification("The settings have been updated successfully.");

            return RedirectToAction("Media");
        }

        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public async Task<IActionResult> ChangeEncryptionKey(GeneralCommonSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var tenantScope = _tenantContext.ActiveTenantScopeConfiguration;
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(tenantScope);

            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = string.Empty;

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (string.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new DefaultException("Encryption private key must be 16 characters long.");

                var oldEncryptionPrivateKey = securitySettings.EncryptionKey;
                if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                    throw new DefaultException("The new encryption key is the same as the old one.");

                //update password information
                var userPasswords = _userService.GetUserPasswords(passwordFormat: PasswordFormat.Encrypted);
                foreach (var userPassword in userPasswords)
                {
                    var decryptedPassword = _encryptionService.DecryptText(userPassword.Password, oldEncryptionPrivateKey);
                    var encryptedPassword = _encryptionService.EncryptText(decryptedPassword, newEncryptionPrivateKey);

                    userPassword.Password = encryptedPassword;
                    _userService.UpdateUserPassword(userPassword);
                }

                securitySettings.EncryptionKey = newEncryptionPrivateKey;
                _settingService.SaveSetting(securitySettings);

                _notificationService.SuccessNotification("Encryption key changed");
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification(ex);
            }

            return RedirectToAction("GeneralCommon");
        }

        #endregion
    }
}