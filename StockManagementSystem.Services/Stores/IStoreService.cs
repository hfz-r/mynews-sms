using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Stores
{
    public interface IStoreService
    {
        Task<IList<Store>> GetStores();

        Store GetStoreById(int storeId);

        Task DeleteStore(Store store);

        Task DeleteStore(IList<Store> stores);

        Task UpdateStore(Store store);

        Task InsertStore(Store store);

        Task<IPagedList<Store>> GetAllStores(string storeName = null, string areaCode = null,
            string city = null, string state = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<User>> GetUsersStore(int? storeId = null, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}