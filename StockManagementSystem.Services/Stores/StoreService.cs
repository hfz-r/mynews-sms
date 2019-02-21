using System;
using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Services.Stores
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepository;

        public StoreService(IRepository<Store> storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<IList<Store>> GetStoresAsync()
        {
            var store = await _storeRepository.Table.ToListAsync();
            return store;
        }

        public List<Store> GetStores()
        {
            var store = _storeRepository.Table.ToList();
            return store;
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
    }
}