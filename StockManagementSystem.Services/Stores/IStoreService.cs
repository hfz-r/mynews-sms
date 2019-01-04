using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagementSystem.Services.Stores
{
    public interface IStoreService
    {
        Task<IList<Store>> GetStoresAsync();
    }
}