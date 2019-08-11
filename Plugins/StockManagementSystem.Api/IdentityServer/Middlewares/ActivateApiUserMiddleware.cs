using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.IdentityServer.Middlewares
{
    public class ActivateApiUserMiddleware : IMiddleware
    {
        private readonly IWorkContext _workContext;
        private readonly IUserService _userService;

        public ActivateApiUserMiddleware(IWorkContext workContext, IUserService userService)
        {
            _workContext = workContext;
            _userService = userService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value.StartsWith("/api/", StringComparison.InvariantCultureIgnoreCase))
            {
                var apiUser = await _userService.GetUserBySystemNameAsync(UserDefaults.ApiUserName);
                if (apiUser != null)
                {
                    _workContext.CurrentUser = apiUser;
                }
            }

            await next(context);
        }
    }
}