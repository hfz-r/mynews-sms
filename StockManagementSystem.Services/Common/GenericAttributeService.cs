using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Data.Extensions;

namespace StockManagementSystem.Services.Common
{
    public partial class GenericAttributeService : IGenericAttributeService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;

        public GenericAttributeService(ICacheManager cacheManager,
            IRepository<GenericAttribute> genericAttributeRepository)
        {
            _cacheManager = cacheManager;
            _genericAttributeRepository = genericAttributeRepository;
        }

        /// <summary>
        /// Deletes an attribute
        /// </summary>
        public async Task DeleteAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            await _genericAttributeRepository.DeleteAsync(attribute);

            _cacheManager.RemoveByPattern(CommonDefaults.GenericAttributePatternCacheKey);
        }

        /// <summary>
        /// Updates the attribute
        /// </summary>
        public async Task UpdateAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            await _genericAttributeRepository.UpdateAsync(attribute);

            _cacheManager.RemoveByPattern(CommonDefaults.GenericAttributePatternCacheKey);
        }

        /// <summary>
        /// Inserts an attribute
        /// </summary>
        public async Task InsertAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            await _genericAttributeRepository.InsertAsync(attribute);

            _cacheManager.RemoveByPattern(CommonDefaults.GenericAttributePatternCacheKey);
        }

        /// <summary>
        /// Get attributes
        /// </summary>
        public async Task<IList<GenericAttribute>> GetAttributesForEntityAsync(int entityId, string keyGroup)
        {
            var key = string.Format(CommonDefaults.GenericAttributeCacheKey, entityId, keyGroup);
            return await _cacheManager.Get(key, async () =>
            {
                var query = from ga in _genericAttributeRepository.Table
                    where ga.EntityId == entityId && ga.KeyGroup == keyGroup
                    select ga;
                var attributes = await query.ToListAsync();
                return attributes;
            });
        }

        /// <summary>
        /// Save attribute value
        /// </summary>
        public async Task SaveAttributeAsync<T>(BaseEntity entity, string key, T value)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var keyGroup = entity.GetUnproxiedEntityType().Name;

            var props = await GetAttributesForEntityAsync(entity.Id, keyGroup);
            var prop = props.FirstOrDefault(ga => ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            var valueStr = CommonHelper.To<string>(value);

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(valueStr))
                {
                    //delete
                    await DeleteAttribute(prop);
                }
                else
                {
                    //update
                    prop.Value = valueStr;
                    await UpdateAttribute(prop);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(valueStr))
                    return;

                prop = new GenericAttribute
                {
                    EntityId = entity.Id,
                    Key = key,
                    KeyGroup = keyGroup,
                    Value = valueStr
                };

                await InsertAttribute(prop);
            }
        }

        /// <summary>
        /// Get an attribute of an entity
        /// </summary>
        public async Task<T> GetAttributeAsync<T>(BaseEntity entity, string key)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var keyGroup = entity.GetUnproxiedEntityType().Name;

            var props = await GetAttributesForEntityAsync(entity.Id, keyGroup);
            if (!props.Any())
                return default(T);

            var prop = props.FirstOrDefault(ga => ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            if (prop == null || string.IsNullOrEmpty(prop.Value))
                return default(T);

            return CommonHelper.To<T>(prop.Value);
        }
    }
}