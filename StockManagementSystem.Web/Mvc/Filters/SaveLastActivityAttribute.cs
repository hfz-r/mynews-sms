using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Web.Mvc.Filters
{
    public class SaveLastActivityAttribute : TypeFilterAttribute
    {
        public SaveLastActivityAttribute() : base(typeof(SaveLastActivityFilter))
        {
        }

        private class SaveLastActivityFilter : IAsyncActionFilter
        {
            private readonly IUserService _userService;
            private readonly IWorkContext _workContext;

            public SaveLastActivityFilter(IUserService userService, IWorkContext workContext)
            {
                _userService = userService;
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

                //update last activity date
                if (_workContext.CurrentUser.LastActivityDateUtc.AddMinutes(1.0) < DateTime.UtcNow)
                {
                    _workContext.CurrentUser.LastActivityDateUtc = DateTime.UtcNow;
                    await _userService.UpdateUserAsync(_workContext.CurrentUser);
                }

                await next();
            }
        }
    }
}