using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Security
{
    public interface IAclService
    {
        Task<bool> Authorize<T>(T entity) where T : BaseEntity, IAclSupported;

        Task<bool> Authorize<T>(T entity, User user) where T : BaseEntity, IAclSupported;

        Task DeleteAclRecord(AclRecord aclRecord);

        Task<AclRecord> GetAclRecordById(int aclRecordId);

        Task<IList<AclRecord>> GetAclRecords<T>(T entity) where T : BaseEntity, IAclSupported;

        Task<int[]> GetRoleIdsWithAccess<T>(T entity) where T : BaseEntity, IAclSupported;

        Task InsertAclRecord(AclRecord aclRecord);

        Task InsertAclRecord<T>(T entity, int roleId) where T : BaseEntity, IAclSupported;

        Task UpdateAclRecord(AclRecord aclRecord);
    }
}