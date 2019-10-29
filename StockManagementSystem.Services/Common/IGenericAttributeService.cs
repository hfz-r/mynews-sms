using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Common;

namespace StockManagementSystem.Services.Common
{
    public interface IGenericAttributeService
    {
        Task DeleteAttribute(GenericAttribute attribute);

        Task DeleteAttributes(IList<GenericAttribute> attributes);

        Task<T> GetAttributeAsync<T>(BaseEntity entity, string key, int tenantId = 0);

        Task<IList<GenericAttribute>> GetAttributesForEntityAsync(int entityId, string keyGroup);

        Task InsertAttribute(GenericAttribute attribute);

        Task SaveAttributeAsync<T>(BaseEntity entity, string key, T value, int tenantId = 0);

        Task UpdateAttribute(GenericAttribute attribute);

        #region Synchronous wrapper

        T GetAttribute<T>(BaseEntity entity, string key, int tenantId = 0);

        #endregion
    }
}