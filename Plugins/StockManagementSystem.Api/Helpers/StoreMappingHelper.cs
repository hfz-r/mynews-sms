using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Services.Stores;

namespace StockManagementSystem.Api.Helpers
{
    public class StoreMappingHelper : IStoreMappingHelper
    {
        private const string StoreIdKey = "store.id-{0}";

        private readonly IStoreService _storeService;
        private readonly ICacheManager _cacheManager;

        public StoreMappingHelper(IStoreService storeService, ICacheManager cacheManager)
        {
            _storeService = storeService;
            _cacheManager = cacheManager;
        }

        public IList<Store> GetValidStores(List<int> storeIds)
        {
            var task = Task.Run(async () => await _storeService.GetStores());

            return task.Result.Where(store => storeIds != null && storeIds.Contains(store.P_BranchNo)).ToList();
        }

        public Store GetValidStore(int id)
        {
            _cacheManager.RemoveByPattern(StoreIdKey);

            var store = _storeService.GetStoreById(id);

            return store;
        }
    }
}