using System.Collections.Generic;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Api.WebHooks;

namespace StockManagementSystem.Api.Services
{
    public class WebHookService : IWebHookService
    {
        private IWebHookManager _webHookManager;
        private IWebHookSender _webHookSender;
        private IWebHookStore _webHookStore;
        private IWebHookFilterManager _webHookFilterManager;
        private readonly ILogger<WebHookManager> _webHookManagerLogger;
        private readonly ILogger<ApiWebHookSender> _apiWebHookSenderLogger;

        public WebHookService(
            ILogger<WebHookManager> webHookManagerLogger,
            ILogger<ApiWebHookSender> apiWebHookSenderLogger)
        {
            _webHookManagerLogger = webHookManagerLogger;
            _apiWebHookSenderLogger = apiWebHookSenderLogger;
        }

        public IWebHookFilterManager GetWebHookFilterManager()
        {
            if (_webHookFilterManager == null)
            {
                var filterProviders = new List<IWebHookFilterProvider>();
                filterProviders.Add(new FilterProvider());
                _webHookFilterManager = new WebHookFilterManager(filterProviders);
            }

            return _webHookFilterManager;
        }

        public IWebHookManager GetWebHookManager()
        {
            return _webHookManager ?? (_webHookManager = new WebHookManager(GetWebHookStore(), GetWebHookSender(), _webHookManagerLogger));
        }

        public IWebHookSender GetWebHookSender()
        {
            return _webHookSender ?? (_webHookSender = new ApiWebHookSender(_apiWebHookSenderLogger));
        }

        public IWebHookStore GetWebHookStore()
        {
            return _webHookStore ?? (_webHookStore = new MemoryWebHookStore());
        }
    }
}