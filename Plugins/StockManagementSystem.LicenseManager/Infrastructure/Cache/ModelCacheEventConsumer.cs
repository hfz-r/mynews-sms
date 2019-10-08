using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Events;
using StockManagementSystem.LicenseManager.Domain;
using StockManagementSystem.Services.Events;

namespace StockManagementSystem.LicenseManager.Infrastructure.Cache
{
    public class ModelCacheEventConsumer : 
        IConsumer<EntityInsertedEvent<License>>,
        IConsumer<EntityUpdatedEvent<License>>, 
        IConsumer<EntityDeletedEvent<License>>
    {
        //Key for caching all licenses
        public const string ALL_LICENSES_MODEL_KEY = "Plugins.licensemanager.all";
        public const string LICENSE_ALL_KEY = "Plugins.licensemanager.all-{0}-{1}";
        public const string LICENSE_PATTERN_KEY = "Plugins.licensemanager.";

        private readonly IStaticCacheManager _cacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityInsertedEvent<License> eventMessage)
        {
            _cacheManager.RemoveByPattern(LICENSE_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<License> eventMessage)
        {
            _cacheManager.RemoveByPattern(LICENSE_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeletedEvent<License> eventMessage)
        {
            _cacheManager.RemoveByPattern(LICENSE_PATTERN_KEY);
        }
    }
}