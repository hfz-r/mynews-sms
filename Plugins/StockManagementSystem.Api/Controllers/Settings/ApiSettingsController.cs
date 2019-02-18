using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Core;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;
using System.Threading.Tasks;
using StockManagementSystem.Api.Models.ApiSettings;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;

namespace StockManagementSystem.Api.Controllers.Settings
{
    public class ApiSettingsController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IUserActivityService _userActivityService;
        private readonly INotificationService _notificationService;
        private readonly IApiSettingModelFactory _apiSettingModelFactory;
        private readonly IPermissionService _permissionService;

        public ApiSettingsController(
            IWorkContext workContext,
            IStoreContext storeContext,
            ISettingService settingService, 
            IUserActivityService userActivityService, 
            INotificationService notificationService,
            IApiSettingModelFactory apiSettingModelFactory,
            IPermissionService permissionService)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _settingService = settingService;
            _userActivityService = userActivityService;
            _notificationService = notificationService;
            _apiSettingModelFactory = apiSettingModelFactory;
            _permissionService = permissionService;
        }

        public async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //prepare model
            var model = await _apiSettingModelFactory.PrepareApiSettingsModel();

            return View(ViewNames.ApiSettings, model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ApiConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var apiSettings = model.ToEntity();

            await _settingService.SaveSettingOverridablePerStore(apiSettings, x => x.EnableApi,
                model.EnableApi_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStore(apiSettings, x => x.EnableLogging,
                model.EnableLogging_OverrideForStore, storeScope, false);

            _settingService.ClearCache();

            //activity log
            await _userActivityService.InsertActivityAsync("EditApiSettings", "Edited api settings");

            _notificationService.SuccessNotification("The settings have been updated successfully.");

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("Index");
        }
    }
}