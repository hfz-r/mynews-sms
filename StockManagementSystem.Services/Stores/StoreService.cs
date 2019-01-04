using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Services.Stores
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepository;

        public StoreService(
            IRepository<Store> storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<IList<Store>> GetStoresAsync()
        {
            var store = await _storeRepository.Table.ToListAsync();
            return store;
        }
    }
}