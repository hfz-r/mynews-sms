using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Api.Helpers
{
    public interface IStoreMappingHelper
    {
        Store GetValidStore(int id);

        IList<Store> GetValidStores(List<int> storeIds);
    }
}