using System;
using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Stores
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ICacheManager _cacheManager;

        public StoreService(
            IRepository<Store> storeRepository,
            IRepository<User> userRepository,
            ICacheManager cacheManager)
        {
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _cacheManager = cacheManager;
        }

        public async Task<IList<Store>> GetStores()
        {
            var store = await _storeRepository.Table.ToListAsync();
            return store;
        }

        public async Task UpdateStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            await _storeRepository.UpdateAsync(store);

            _cacheManager.RemoveByPattern(StoreDefaults.StoresPatternCacheKey);
        }

        public async Task DeleteStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            await _storeRepository.DeleteAsync(store);
        }

        public async Task DeleteStore(IList<Store> stores)
        {
            if (stores == null)
                throw new ArgumentNullException(nameof(stores));

            await _storeRepository.DeleteAsync(stores);
        }

        public async Task InsertStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            await _storeRepository.InsertAsync(store);

            _cacheManager.RemoveByPattern(StoreDefaults.StoresPatternCacheKey);
        }

        public virtual Store GetStoreById(int storeId)
        {
            var key = string.Format(StoreDefaults.StoresByIdCacheKey, storeId);
            return _cacheManager.Get(key, () => _storeRepository.GetById(storeId));
        }

        public async Task<IPagedList<Store>> GetAllStores(string storeName = null, string areaCode = null,
            string city = null, string state = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _storeRepository.Table;

            //filter by name
            if (!string.IsNullOrEmpty(storeName))
                query = query.Where(store => store.P_Name.Contains(storeName));

            //filter by area code
            if (areaCode != "0")
                query = query.Where(store => store.P_AreaCode.Contains(areaCode));

            //filter by cities
            if (city != "0")
                query = query.Where(store => store.P_City.Contains(city));

            //filter by states
            if (state != "0")
                query = query.Where(store => store.P_State.Contains(state));

            query = query.OrderBy(store => store.P_Name).ThenBy(store => store.P_AreaCode);

            return await Task.FromResult<IPagedList<Store>>(new PagedList<Store>(query, pageIndex, pageSize));
        }

        public async Task<IPagedList<User>> GetUsersStore(int? storeId = null,int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var users = _userRepository.Table;

            if (storeId.HasValue)
                users = users.Where(user => user.UserStores.Any(mapping => mapping.StoreId == storeId.Value));

            users = users.OrderBy(user => user.Id);

            return await Task.FromResult<IPagedList<User>>(new PagedList<User>(users, pageIndex, pageSize));
        }
    }
}