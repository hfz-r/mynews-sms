using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Data.Extensions;

namespace StockManagementSystem.Services.Stores
{
    /// <summary>
    /// Store mapping service
    /// </summary>
    public class StoreMappingService : IStoreMappingService
    {
        private readonly RecordSettings _recordSettings;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        public StoreMappingService(
            RecordSettings recordSettings, 
            IRepository<StoreMapping> storeMappingRepository, 
            IStaticCacheManager cacheManager, 
            IWorkContext workContext)
        {
            _recordSettings = recordSettings;
            _storeMappingRepository = storeMappingRepository;
            _cacheManager = cacheManager;
            _workContext = workContext;
        }

        public async Task DeleteStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentNullException(nameof(storeMapping));

            await _storeMappingRepository.DeleteAsync(storeMapping);

            _cacheManager.RemoveByPattern(StoreDefaults.StoreMappingPatternCacheKey);
        }

        public async Task<StoreMapping> GetStoreMappingById(int storeMappingId)
        {
            if (storeMappingId == 0)
                return null;

            return await _storeMappingRepository.GetByIdAsync(storeMappingId);
        }

        public async Task<IList<StoreMapping>> GetStoreMappings<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var query = from sm in _storeMappingRepository.Table
                where sm.EntityId == entityId && sm.EntityName == entityName
                select sm;

            var storeMappings = await query.ToListAsync();

            return storeMappings;
        }

        protected async Task InsertStoreMapping(StoreMapping storeMapping)
        {
            if (storeMapping == null)
                throw new ArgumentNullException(nameof(storeMapping));

            await _storeMappingRepository.InsertAsync(storeMapping);

            _cacheManager.RemoveByPattern(StoreDefaults.StoreMappingPatternCacheKey);
        }

        public async Task InsertStoreMapping<T>(T entity, int storeId) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (storeId == 0)
                throw new ArgumentOutOfRangeException(nameof(storeId));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var storeMapping = new StoreMapping
            {
                EntityId = entityId,
                EntityName = entityName,
                StoreId = storeId
            };

            await InsertStoreMapping(storeMapping);
        }

        /// <summary>
        /// Find store identifiers with granted access (mapped to the entity)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Store identifiers</returns>
        public virtual int[] GetStoresIdsWithAccess<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var key = string.Format(StoreDefaults.StoreMappingByEntityIdNameCacheKey, entityId, entityName);
            return _cacheManager.Get(key, () =>
            {
                var query = from sm in _storeMappingRepository.Table
                    where sm.EntityId == entityId &&
                          sm.EntityName == entityName
                    select sm.StoreId;
                return query.ToArray();
            });
        }

        /// <summary>
        /// Authorize whether entity could be accessed in the current store (mapped to this store)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity) where T : BaseEntity, IStoreMappingSupported
        {
            return Authorize(entity, _workContext.CurrentUser);
        }

        /// <summary>
        /// Authorize whether entity could be accessed in a store (mapped to this store)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="user">User entity</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize<T>(T entity, User user) where T : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                return false;

            if (user == null)
                return false;

            if (_recordSettings.IgnoreStoreLimitations)
                return true;

            if (!entity.LimitedToStores)
                return true;

            foreach (var store1 in user.AppliedStores.Where(s => s.Active))
            foreach (var store2Id in GetStoresIdsWithAccess(entity))
                if (store1.P_BranchNo == store2Id)
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }
    }
}