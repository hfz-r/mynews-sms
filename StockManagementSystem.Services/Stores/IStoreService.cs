using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagementSystem.Services.Stores
{
    public interface IStoreService
    {
     	void DeleteStore(Store store);
     	
        void DeleteStore(List<Store> stores);
       
        void UpdateStore(Store store);
        
        Task InsertStore(Store store);

        Task<IList<Store>> GetStoresAsync(bool loadCacheableCopy = true);

        Task<Store> GetStoreByBranchNoAsync(int branchNo, bool loadCacheableCopy = true);

        string[] ParseHostValues(Store store);

        bool ContainsHostValue(Store store, string host);
    }
}