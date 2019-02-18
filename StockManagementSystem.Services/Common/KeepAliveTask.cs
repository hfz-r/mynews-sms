using System.Net;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Http;
using StockManagementSystem.Services.Tasks;

namespace StockManagementSystem.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public class KeepAliveTask : IScheduleTask
    {
        private readonly IWebHelper _webHelper;

        public KeepAliveTask(IWebHelper webHelper)
        {
            this._webHelper = webHelper;
        }

        public void Execute()
        {
            var keepAliveUrl = $"{_webHelper.GetStoreLocation()}{HttpDefaults.KeepAlivePath}";
            using (var wc = new WebClient())
            {
                wc.DownloadString(keepAliveUrl);
            }
        }
    }
}