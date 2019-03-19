using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Web.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that validates IP address
    /// </summary>
    public class ValidateIpAddressAttribute : TypeFilterAttribute
    {
        public ValidateIpAddressAttribute() : base(typeof(ValidateIpAddressFilter))
        {
        }

        private class ValidateIpAddressFilter : IActionFilter
        {
            private readonly IWebHelper _webHelper;
            private readonly SecuritySettings _securitySettings;

            public ValidateIpAddressFilter(IWebHelper webHelper, SecuritySettings securitySettings)
            {
                _webHelper = webHelper;
                _securitySettings = securitySettings;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var actionName = actionDescriptor?.ActionName;
                var controllerName = actionDescriptor?.ControllerName;

                if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                    return;

                //get allowed IP addresses
                var ipAddresses = _securitySettings.AllowedIpAddresses;

                if (ipAddresses == null || !ipAddresses.Any())
                    return;

                //whether current IP is allowed
                var currentIp = _webHelper.GetCurrentIpAddress();
                if (ipAddresses.Any(ip => ip.Equals(currentIp, StringComparison.InvariantCultureIgnoreCase)))
                    return;

                //ensure that it's not 'Access denied' page
                if (!(controllerName.Equals("Security", StringComparison.InvariantCultureIgnoreCase) &&
                      actionName.Equals("AccessDenied", StringComparison.InvariantCultureIgnoreCase)))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Security", context.RouteData.Values);
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }
    }
}