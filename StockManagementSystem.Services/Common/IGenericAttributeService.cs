using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Common;

namespace StockManagementSystem.Services.Common
{
    public interface IGenericAttributeService
    {
        Task DeleteAttribute(GenericAttribute attribute);

        Task<T> GetAttributeAsync<T>(BaseEntity entity, string key, int storeId = 0);

        Task<IList<GenericAttribute>> GetAttributesForEntityAsync(int entityId, string keyGroup);

        Task InsertAttribute(GenericAttribute attribute);

        Task SaveAttributeAsync<T>(BaseEntity entity, string key, T value, int storeId = 0);

        Task UpdateAttribute(GenericAttribute attribute);
    }
}