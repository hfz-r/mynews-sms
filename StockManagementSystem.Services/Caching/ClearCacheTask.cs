using StockManagementSystem.Core.Caching;
using StockManagementSystem.Services.Tasks;

namespace StockManagementSystem.Services.Caching
{
    /// <summary>
    /// Clear cache scheduled task implementation
    /// </summary>
    public class ClearCacheTask : IScheduleTask
    {
        private readonly IStaticCacheManager _staticCacheManager;

        public ClearCacheTask(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        public void Execute()
        {
            _staticCacheManager.Clear();
        }
    }
}