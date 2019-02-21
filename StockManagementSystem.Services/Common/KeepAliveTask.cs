using System.Net;
using System.Threading;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Http;
using StockManagementSystem.Services.Tasks.Scheduling;

namespace StockManagementSystem.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public class KeepAliveTask : IScheduledTask
    {
        private readonly IWebHelper _webHelper;

        public KeepAliveTask(IWebHelper webHelper)
        {
            this._webHelper = webHelper;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var keepAliveUrl = $"{_webHelper.GetLocation()}{HttpDefaults.KeepAlivePath}";

            using (var wc = new WebClient())
            {
                await wc.DownloadStringTaskAsync(keepAliveUrl);
            }
        }

        public string Schedule => "*/5 * * * *";
    }
}