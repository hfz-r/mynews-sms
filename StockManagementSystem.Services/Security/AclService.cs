using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Data.Extensions;

namespace StockManagementSystem.Services.Security
{
    /// <summary>
    /// Represent access control list 
    /// </summary>
    /// <remarks>Will be used by modules which affected by role</remarks>
    public class AclService : IAclService
    {
        private readonly IRepository<AclRecord> _aclRecordRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;

        public AclService(IRepository<AclRecord> aclRecordRepository, IStaticCacheManager staticCacheManager,
            IWorkContext workContext)
        {
            _aclRecordRepository = aclRecordRepository;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
        }

        public Task DeleteAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Delete(aclRecord);

            _staticCacheManager.RemoveByPattern(SecurityDefaults.AclRecordPatternCacheKey);

            return Task.CompletedTask;
        }

        public Task<AclRecord> GetAclRecordById(int aclRecordId)
        {
            if (aclRecordId == 0)
                return null;

            return Task.FromResult(_aclRecordRepository.GetById(aclRecordId));
        }

        public async Task<IList<AclRecord>> GetAclRecords<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var query = from ur in _aclRecordRepository.Table
                where ur.EntityId == entityId && ur.EntityName == entityName
                select ur;
            var aclRecords = await query.ToListAsync();
            return aclRecords;
        }

        public Task InsertAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Insert(aclRecord);

            _staticCacheManager.RemoveByPattern(SecurityDefaults.AclRecordPatternCacheKey);

            return Task.CompletedTask;
        }

        public async Task InsertAclRecord<T>(T entity, int roleId) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (roleId == 0)
                throw new ArgumentOutOfRangeException(nameof(roleId));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var aclRecord = new AclRecord
            {
                EntityId = entityId,
                EntityName = entityName,
                RoleId = roleId
            };

            await InsertAclRecord(aclRecord);
        }

        public Task UpdateAclRecord(AclRecord aclRecord)
        {
            if (aclRecord == null)
                throw new ArgumentNullException(nameof(aclRecord));

            _aclRecordRepository.Update(aclRecord);

            _staticCacheManager.RemoveByPattern(SecurityDefaults.AclRecordPatternCacheKey);

            return Task.CompletedTask;
        }

        public async Task<int[]> GetRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityId = entity.Id;
            var entityName = entity.GetUnproxiedEntityType().Name;

            var key = string.Format(SecurityDefaults.AclRecordByEntityIdNameCacheKey, entityId, entityName);
            return await _staticCacheManager.Get(key, async () =>
            {
                var query = from ur in _aclRecordRepository.Table
                    where ur.EntityId == entityId &&
                          ur.EntityName == entityName
                    select ur.RoleId;
                return await query.ToArrayAsync();
            });
        }

        public async Task<bool> Authorize<T>(T entity) where T : BaseEntity, IAclSupported
        {
            return await Authorize(entity, _workContext.CurrentUser);
        }

        public async Task<bool> Authorize<T>(T entity, User user) where T : BaseEntity, IAclSupported
        {
            if (entity == null)
                return false;

            if (user == null)
                return false;

            if (!entity.SubjectToAcl)
                return true;

            foreach (var role in await GetRoleIdsWithAccess(entity))
                if (role != 0)
                    return true;

            return false;
        }
    }
}