using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Common;

namespace StockManagementSystem.Web.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that saves last visited page by user
    /// </summary>
    public class SaveLastVisitedPageAttribute : TypeFilterAttribute
    {
        public SaveLastVisitedPageAttribute() : base(typeof(SaveLastVisitedPageFilter))
        {
        }

        private class SaveLastVisitedPageFilter : IAsyncActionFilter
        {
            private readonly UserSettings _userSettings;
            private readonly IGenericAttributeService _genericAttributeService;
            private readonly IWebHelper _webHelper;
            private readonly IWorkContext _workContext;

            public SaveLastVisitedPageFilter(
                UserSettings userSettings,
                IGenericAttributeService genericAttributeService,
                IWebHelper webHelper,
                IWorkContext workContext)
            {
                _userSettings = userSettings;
                _genericAttributeService = genericAttributeService;
                _webHelper = webHelper;
                _workContext = workContext;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                {
                    await next();
                    return;
                }

                //only in GET requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    await next();
                    return;
                }

                if (!DataSettingsManager.DatabaseIsInstalled)
                {
                    await next();
                    return;
                }

                //check whether we store last visited page URL
                if (!_userSettings.StoreLastVisitedPage)
                {
                    await next();
                    return;
                }

                //get current page
                var pageUrl = _webHelper.GetThisPageUrl(true);
                if (string.IsNullOrEmpty(pageUrl))
                {
                    await next();
                    return;
                }

                //get previous last page
                var previousPageUrl = await _genericAttributeService.GetAttributeAsync<string>(_workContext.CurrentUser,
                    UserDefaults.LastVisitedPageAttribute);

                //save new one if don't match
                if (!pageUrl.Equals(previousPageUrl, StringComparison.InvariantCultureIgnoreCase))
                    await _genericAttributeService.SaveAttributeAsync(_workContext.CurrentUser,
                        UserDefaults.LastVisitedPageAttribute, pageUrl);

                await next();
            }
        }
    }
}