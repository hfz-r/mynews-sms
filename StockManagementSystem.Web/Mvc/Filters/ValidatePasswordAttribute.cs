using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Web.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that validates user password expiration
    /// </summary>
    public class ValidatePasswordAttribute : TypeFilterAttribute
    {
        public ValidatePasswordAttribute() : base(typeof(ValidatePasswordFilter))
        {
        }

        private class ValidatePasswordFilter : IActionFilter
        {
            private readonly IUserService _userService;
            private readonly IUrlHelperFactory _urlHelperFactory;
            private readonly IWorkContext _workContext;

            public ValidatePasswordFilter(IUserService userService, IUrlHelperFactory urlHelperFactory, IWorkContext workContext)
            {
                _userService = userService;
                _urlHelperFactory = urlHelperFactory;
                _workContext = workContext;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //get action and controller names
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var actionName = actionDescriptor?.ActionName;
                var controllerName = actionDescriptor?.ControllerName;

                if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                    return;

                //don't validate on ChangePassword page
                if (!(controllerName.Equals("User", StringComparison.InvariantCultureIgnoreCase) &&
                      actionName.Equals("ChangePassword", StringComparison.InvariantCultureIgnoreCase)))
                {
                    //check password expiration
                    if (_userService.PasswordIsExpired(_workContext.CurrentUser))
                    {
                        //redirect to ChangePassword page if expires
                        var changePasswordUrl = _urlHelperFactory.GetUrlHelper(context).RouteUrl("UserChangePassword");
                        context.Result = new RedirectResult(changePasswordUrl);
                    }
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }

    }
}