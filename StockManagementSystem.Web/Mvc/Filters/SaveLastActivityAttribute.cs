using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockManagementSystem.Core;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Web.Mvc.Filters
{
    public class SaveLastActivityAttribute : TypeFilterAttribute
    {
        public SaveLastActivityAttribute() : base(typeof(SaveLastActivityFilter))
        {
        }

        private class SaveLastActivityFilter : IActionFilter
        {
            private readonly IUserService _userService;
            private readonly IWorkContext _workContext;

            public SaveLastActivityFilter(IUserService userService, IWorkContext workContext)
            {
                _userService = userService;
                _workContext = workContext;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get,
                    StringComparison.InvariantCultureIgnoreCase))
                    return;

                //update last activity date
                if (_workContext.CurrentUser.LastActivityDateUtc.AddMinutes(1.0) < DateTime.UtcNow)
                {
                    _workContext.CurrentUser.LastActivityDateUtc = DateTime.UtcNow;
                    _userService.UpdateUser(_workContext.CurrentUser);
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }
    }
}