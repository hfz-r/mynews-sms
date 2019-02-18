using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Tests.Helpers
{
    public static class ActionResultExecutor
    {
        public static HttpStatusCode ExecuteResult(IActionResult result)
        {
            var actionContext = new ActionContext();
            actionContext.HttpContext = new DefaultHttpContext();

            result.ExecuteResultAsync(actionContext);
            var statusCode = actionContext.HttpContext.Response.StatusCode;

            return (HttpStatusCode) statusCode;
        }
    }
}