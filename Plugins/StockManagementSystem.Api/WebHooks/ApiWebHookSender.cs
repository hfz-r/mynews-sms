using System.Collections.Generic;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace StockManagementSystem.Api.WebHooks
{
    public class ApiWebHookSender : DataflowWebHookSender
    {
        private const string WebHookIdKey = "WebHookId";

        public ApiWebHookSender(ILogger<ApiWebHookSender> logger) : base(logger)
        {
        }

        /// <inheritdoc />
        protected override JObject CreateWebHookRequestBody(WebHookWorkItem workItem)
        {
            JObject data = base.CreateWebHookRequestBody(workItem);

            Dictionary<string, object> body = data.ToObject<Dictionary<string, object>>();
            body[WebHookIdKey] = workItem.WebHook.Id;

            return JObject.FromObject(body);
        }
    }
}