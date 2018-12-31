using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class PreferencesController : BaseController
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        public PreferencesController(IGenericAttributeService genericAttributeService, IWorkContext workContext)
        {
            _genericAttributeService = genericAttributeService;
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
    }
}