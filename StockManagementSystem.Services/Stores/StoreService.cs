using System;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using System.Linq;

namespace StockManagementSystem.Services.Stores
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepository;
        private readonly IStaticCacheManager _cacheManager;

        public StoreService(IRepository<Store> storeRepository, IStaticCacheManager cacheManager)
        {
            _storeRepository = storeRepository;
            _cacheManager = cacheManager;
        }
        
        public virtual void UpdateStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            _storeRepository.Update(store);
        }

        public virtual void DeleteStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            _storeRepository.Delete(store);
        }

        public virtual void DeleteStore(List<Store> stores)
        {
            if (stores == null)
                throw new ArgumentNullException(nameof(stores));

            _storeRepository.Delete(stores);
        }

        public async Task InsertStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            await _storeRepository.InsertAsync(store);
        }

        /// <summary>
        /// Gets all stores
        /// </summary>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached</param>
        /// <returns>Stores</returns>
        public async Task<IList<Store>> GetStoresAsync(bool loadCacheableCopy = true)
        {
            async Task<IList<Store>> LoadStoresFunc()
            {
                var query = from s in _storeRepository.Table orderby s.P_BranchNo select s;
                return await query.ToListAsync();
            }

            if (loadCacheableCopy)
            {
                return _cacheManager.Get(StoreDefaults.StoresAllCacheKey, () =>
                {
                    var result = new List<Store>();
                    foreach (var store in LoadStoresFunc().GetAwaiter().GetResult())
                        result.Add(new StoreForCaching(store));
                    return result;
                });
            }

            return await LoadStoresFunc();
        }

        /// <summary>
        /// Gets a store 
        /// </summary>
        /// <param name="branchNo">Store identifier</param>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached</param>
        /// <returns>Store</returns>
        public async Task<Store> GetStoreByBranchNoAsync(int branchNo, bool loadCacheableCopy = true)
        {
            if (branchNo == 0)
                return null;

            async Task<Store> LoadStoreFunc()
            {
                return await _storeRepository.GetByIdAsync(branchNo);
            }

            if (!loadCacheableCopy)
                return await LoadStoreFunc();

            var key = string.Format(StoreDefaults.StoresByIdCacheKey, branchNo);
            return _cacheManager.Get(key, () =>
            {
                var store = LoadStoreFunc().GetAwaiter().GetResult();
                if (store == null)
                    return null;
                return new StoreForCaching(store);
            });
        }

        #region Store context

        /// <summary>
        /// Parse comma-separated Hosts
        /// </summary>
        /// <param name="store">Store</param>
        /// <returns>Comma-separated hosts</returns>
        public virtual string[] ParseHostValues(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var parsedValues = new List<string>();
            if (string.IsNullOrEmpty(store.Hosts))
                return parsedValues.ToArray();

            var hosts = store.Hosts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var host in hosts)
            {
                var tmp = host.Trim();
                if (!string.IsNullOrEmpty(tmp))
                    parsedValues.Add(tmp);
            }

            return parsedValues.ToArray();
        }

        /// <summary>
        /// Indicates whether a store contains a specified host
        /// </summary>
        /// <param name="store">Store</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        public virtual bool ContainsHostValue(Store store, string host)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (string.IsNullOrEmpty(host))
                return false;

            var contains = this.ParseHostValues(store).Any(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase));

            return contains;
        }

        #endregion
    }
}