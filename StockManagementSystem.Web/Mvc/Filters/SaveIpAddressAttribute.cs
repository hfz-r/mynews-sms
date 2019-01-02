using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockManagementSystem.Core;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Web.Mvc.Filters
{
    public class SaveIpAddressAttribute : TypeFilterAttribute
    {
        public SaveIpAddressAttribute() : base(typeof(SaveIpAddressFilter))
        {
        }

        private class SaveIpAddressFilter : IActionFilter
        {
            private readonly IUserService _userService;
            private readonly IWebHelper _webHelper;
            private readonly IWorkContext _workContext;

            public SaveIpAddressFilter(IUserService userService, IWebHelper webHelper, IWorkContext workContext)
            {
                _userService = userService;
                _webHelper = webHelper;
                _workContext = workContext;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                //only in GET requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                //get current IP address
                var currentIpAddress = _webHelper.GetCurrentIpAddress();
                if (string.IsNullOrEmpty(currentIpAddress))
                    return;

                //update user's IP address
                if (!currentIpAddress.Equals(_workContext.CurrentUser.LastIpAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    _workContext.CurrentUser.LastIpAddress = currentIpAddress;
                    _userService.UpdateUser(_workContext.CurrentUser);
                }

            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }
    }
}