using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagementSystem.Services.Stores
{
    public interface IStoreService
    {
        Task<IList<Store>> GetStoresAsync();
        List<Store> GetStores();
        void DeleteStore(Store store);
        void DeleteStore(List<Store> stores);
        void UpdateStore(Store store);
        Task InsertStore(Store store);
    }
}