using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Builder;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.UI;

namespace StockManagementSystem.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        public override JsonResult Json(object data)
        {
            //use IsoDateFormat on writing JSON text to fix issue with dates in KendoUI grid
            var serializerSettings =
                EngineContext.Current.Resolve<IOptions<MvcJsonOptions>>()?.Value?.SerializerSettings
                ?? new JsonSerializerSettings();

            serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;

            return base.Json(data, serializerSettings);
        }

        protected JsonResult ErrorForKendoGridJson(string errorMessage)
        {
            var gridModel = new DataSourceResult
            {
                Errors = errorMessage
            };

            return Json(gridModel);
        }

        protected virtual void DisplayEditLink(string editPageUrl)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();

            pageHeadBuilder.AddEditPageUrl(editPageUrl);
        }

        protected virtual IActionResult AccessDeniedView()
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = webHelper.GetRawUrl(this.Request) });
        }

        protected JsonResult AccessDeniedKendoGridJson()
        {
            return ErrorForKendoGridJson("You do not have permission to perform the selected operation.");
        }
    }
}