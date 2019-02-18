using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;

namespace StockManagementSystem.Api.IdentityServer.Middlewares
{
    public class ScopeParametersMiddleware
    {
        private readonly RequestDelegate _next;

        public ScopeParametersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Equals("/connect/authorize", StringComparison.InvariantCultureIgnoreCase) ||
                context.Request.Path.Value.Equals("/oauth/authorize", StringComparison.InvariantCultureIgnoreCase))
            {
                // Make sure we have "sms_api" and "offline_access" scope
                var queryValues = new Dictionary<string, StringValues>();

                foreach (var query in context.Request.Query)
                {
                    if (query.Key == "scope")
                    {
                        string scopeValue = query.Value;
                        if (!scopeValue.Contains("sms_api offline_access"))
                        {
                            // add our scope instead since we don't support other scopes
                            queryValues.Add(query.Key, "sms_api offline_access");
                            continue;
                        }
                    }

                    queryValues.Add(query.Key, query.Value);
                }

                if (!queryValues.ContainsKey("scope"))
                {
                    // if no scope is specified we add it
                    queryValues.Add("scope", "sms_api offline_access");
                }

                var newQueryCollection = new QueryCollection(queryValues);
                context.Request.Query = newQueryCollection;
            }

            await _next(context);
        }
    }
}