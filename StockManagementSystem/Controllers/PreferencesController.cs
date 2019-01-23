using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class PreferencesController : BaseController
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        public PreferencesController(
            IGenericAttributeService genericAttributeService,
            IStaticCacheManager cacheManager,
            IWorkContext workContext)
        {
            _genericAttributeService = genericAttributeService;
            _cacheManager = cacheManager;
            _workContext = workContext;
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
    }
}