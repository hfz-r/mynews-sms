using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Common;

namespace StockManagementSystem.Services.Common
{
    public interface IGenericAttributeService
    {
        Task DeleteAttribute(GenericAttribute attribute);

        Task<T> GetAttributeAsync<T>(BaseEntity entity, string key);

        Task<IList<GenericAttribute>> GetAttributesForEntityAsync(int entityId, string keyGroup);

        Task InsertAttribute(GenericAttribute attribute);

        Task SaveAttributeAsync<T>(BaseEntity entity, string key, T value);

        Task UpdateAttribute(GenericAttribute attribute);
    }
}