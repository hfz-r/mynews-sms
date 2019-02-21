using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Core.Http
{
    public class InstallUrlMiddleware
    {
        private readonly RequestDelegate _next;

        public InstallUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IWebHelper webHelper)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
            {
                var installUrl = $"{webHelper.GetLocation()}{HttpDefaults.InstallPath}";
                if (!webHelper.GetThisPageUrl(false)
                    .StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    //redirect
                    context.Response.Redirect(installUrl);
                    return;
                }
            }

            await _next(context);
        }
    }
}