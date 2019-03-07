using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Stores
{
    public interface IStoreMappingService
    {
        int[] GetStoresIdsWithAccess<T>(T entity) where T : BaseEntity, IStoreMappingSupported;

        bool Authorize<T>(T entity) where T : BaseEntity, IStoreMappingSupported;

        bool Authorize<T>(T entity, User user) where T : BaseEntity, IStoreMappingSupported;

        Task DeleteStoreMapping(StoreMapping storeMapping);

        Task<StoreMapping> GetStoreMappingById(int storeMappingId);

        Task<IList<StoreMapping>> GetStoreMappings<T>(T entity) where T : BaseEntity, IStoreMappingSupported;

        Task InsertStoreMapping<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported;
    }
}