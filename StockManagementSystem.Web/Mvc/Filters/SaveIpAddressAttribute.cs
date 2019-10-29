using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Web.Mvc.Filters
{
    public class SaveIpAddressAttribute : TypeFilterAttribute
    {
        public SaveIpAddressAttribute() : base(typeof(SaveIpAddressFilter))
        {
        }

        private class SaveIpAddressFilter : IAsyncActionFilter
        {
            private readonly IUserService _userService;
            private readonly IWebHelper _webHelper;
            private readonly IWorkContext _workContext;
            private readonly UserSettings _userSettings;

            public SaveIpAddressFilter(
                IUserService userService,
                IWebHelper webHelper,
                IWorkContext workContext,
                UserSettings userSettings)
            {
                _userService = userService;
                _webHelper = webHelper;
                _workContext = workContext;
                _userSettings = userSettings;
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

                //check whether we store IP addresses
                if (!_userSettings.StoreIpAddresses)
                {
                    await next();
                    return;
                }

                //get current IP address
                var currentIpAddress = _webHelper.GetCurrentIpAddress();
                if (string.IsNullOrEmpty(currentIpAddress))
                {
                    await next();
                    return;
                }

                //update user's IP address
                if (!currentIpAddress.Equals(_workContext.CurrentUser.LastIpAddress,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    _workContext.CurrentUser.LastIpAddress = currentIpAddress;
                    await _userService.UpdateUserAsync(_workContext.CurrentUser);
                }

                await next();
            }
        }
    }
}