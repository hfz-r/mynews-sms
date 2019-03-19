using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class CommonController : BaseController
    {
        private readonly CommonSettings _commonSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        public CommonController(
            CommonSettings commonSettings,
            IGenericAttributeService genericAttributeService,
            IStaticCacheManager cacheManager,
            IWorkContext workContext,
            ILogger logger)
        {
            _commonSettings = commonSettings;
            _genericAttributeService = genericAttributeService;
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
    }
}