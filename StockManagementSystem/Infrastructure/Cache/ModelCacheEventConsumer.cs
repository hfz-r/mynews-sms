using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Configuration;
using StockManagementSystem.Core.Events;
using StockManagementSystem.Services.Events;

namespace StockManagementSystem.Infrastructure.Cache
{
    public partial class ModelCacheEventConsumer :
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>
    {
        private readonly IStaticCacheManager _cacheManager;

        public ModelCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            _cacheManager.RemoveByPattern(ModelCacheDefaults.LogoPathPatternKey); //depends on CommonSettings.LogoPictureId
        }
    }
}