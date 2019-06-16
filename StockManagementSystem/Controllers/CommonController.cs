using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Common;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class CommonController : BaseController
    {
        private readonly CommonSettings _commonSettings;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        public CommonController(
            CommonSettings commonSettings,
            ICommonModelFactory commonModelFactory,
            IGenericAttributeService genericAttributeService,
            IPermissionService permissionService,
            IStaticCacheManager cacheManager,
            IWorkContext workContext,
            ILogger logger)
        {
            _commonSettings = commonSettings;
            _commonModelFactory = commonModelFactory;
            _genericAttributeService = genericAttributeService;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _workContext = workContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<JsonResult> SavePreference(string name, bool value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            await _genericAttributeService.SaveAttributeAsync(_workContext.CurrentUser, name, value);

            return Json(new {Result = true});
        }

        [HttpPost]
        public IActionResult ClearCache(string returnUrl = "")
        {
            _cacheManager.Clear();

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home");

            return Redirect(returnUrl);
        }

        public IActionResult PageNotFound()
        {
            if (_commonSettings.Log404Errors)
            {
                var statusCodeReExecuteFeature = HttpContext?.Features?.Get<IStatusCodeReExecuteFeature>();
                _logger.Error($"Error 404. The requested page ({statusCodeReExecuteFeature?.OriginalPath}) was not found",
                    user: _workContext.CurrentUser);
            }

            Response.StatusCode = 404;
            Response.ContentType = "text/html";

            return View();
        }

        public async Task<IActionResult> SystemInfo()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = await _commonModelFactory.PrepareSystemInfoModel(new SystemInfoModel());

            return View(model);
        }

        public async Task<IActionResult> Warnings()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = await _commonModelFactory.PrepareSystemWarningModels();

            return View(model);
        }
    }
}