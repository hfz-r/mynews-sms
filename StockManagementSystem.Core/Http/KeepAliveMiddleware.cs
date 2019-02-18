using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Core.Http
{
    /// <summary>
    /// Represents middleware that checks whether request is for keep alive
    /// </summary>
    public class KeepAliveMiddleware
    {
        private readonly RequestDelegate _next;

        public KeepAliveMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IWebHelper webHelper)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
            {
                var keepAliveUrl = $"{webHelper.GetStoreLocation()}{HttpDefaults.KeepAlivePath}";

                if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
                    return;
            }

            await _next(context);
        }
    }
}