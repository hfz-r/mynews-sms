using System.Threading;
using System.Threading.Tasks;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Services.Tasks.Scheduling;

namespace StockManagementSystem.Services.Caching
{
    /// <summary>
    /// Represent a task to clear cache
    /// </summary>
    public class ClearCacheTask : IScheduledTask
    {
        private readonly IStaticCacheManager _staticCacheManager;

        public ClearCacheTask(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _staticCacheManager.Clear();

            await Task.Delay(5000, cancellationToken);
        }

        public string Schedule => "*/30 * * * *";

        public bool Enabled => true;
    }
}