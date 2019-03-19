using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Mvc.Filters;
using StockManagementSystem.Web.Security;
using StockManagementSystem.Web.UI;

namespace StockManagementSystem.Web.Controllers
{
    //[HttpsRequirement(SslRequirement.Yes)]
    [HttpsRequirement(SslRequirement.NoMatter)]
    [AntiForgery]
    [ValidateIpAddress]
    [ValidatePassword]
    [AuthorizeUser]
    [SaveIpAddress]
    [SaveLastActivity]
    [SaveLastVisitedPage]
    public abstract class BaseController : Controller
    {
        #region Notifications

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

        #endregion

        #region Security

        /// <summary>
        /// Access denied view
        /// </summary>
        protected virtual IActionResult AccessDeniedView()
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            return RedirectToAction("AccessDenied", "Security", new {pageUrl = webHelper.GetRawUrl(this.Request)});
        }

        /// <summary>
        /// Access denied JSON data for kendo grid
        /// </summary>
        protected JsonResult AccessDeniedKendoGridJson()
        {
            return ErrorForKendoGridJson("You do not have permission to perform the selected operation.");
        }

        #endregion

        #region Tab-helper

        //Save selected tab name
        protected virtual void SaveSelectedTabName(string tabName = "", bool persistForTheNextRequest = true)
        {
            //default root tab
            SaveSelectedTabName(tabName, "selected-tab-name", null, persistForTheNextRequest);

            //Form is available for POST only
            if (!Request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase))
                return;

            foreach (var key in Request.Form.Keys)
            {
                if (key.StartsWith("selected-tab-name-", StringComparison.InvariantCultureIgnoreCase))
                    SaveSelectedTabName(null, key, key.Substring("selected-tab-name-".Length), persistForTheNextRequest);
            }
        }

        protected virtual void SaveSelectedTabName(string tabName, string formKey, string dataKeyPrefix, bool persistForTheNextRequest)
        {
            //keep this method synchronized with
            //"GetSelectedTabName" method of \StockManagementSystem.Web\Extensions\HtmlExtensions.cs
            if (string.IsNullOrEmpty(tabName))
            {
                tabName = Request.Form[formKey];
            }

            if (string.IsNullOrEmpty(tabName))
                return;

            var dataKey = "mynsms.selected-tab-name";
            if (!string.IsNullOrEmpty(dataKeyPrefix))
                dataKey += $"-{dataKeyPrefix}";

            if (persistForTheNextRequest)
            {
                TempData[dataKey] = tabName;
            }
            else
            {
                ViewData[dataKey] = tabName;
            }
        }

        #endregion

        #region Serializer

        /// <summary>
        /// Creates an object that serializes the specified object to JSON.
        /// </summary>
        public override JsonResult Json(object data)
        {
            var useIsoDateFormat = EngineContext.Current.Resolve<CommonSettings>()?.UseIsoDateFormatInJsonResult ?? false;
            var serializerSettings = EngineContext.Current.Resolve<IOptions<MvcJsonOptions>>()?.Value?.SerializerSettings 
                ?? new JsonSerializerSettings();

            if (!useIsoDateFormat)
                return base.Json(data, serializerSettings);

            serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;

            return base.Json(data, serializerSettings);
        }

        #endregion
    }
}