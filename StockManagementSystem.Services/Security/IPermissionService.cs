using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Security
{
    public interface IPermissionService
    {
        Task<bool> AuthorizeAsync(Permission permission);

        Task<bool> AuthorizeAsync(Permission permission, User user);

        Task<bool> AuthorizeAsync(string permissionSystemName);

        Task<bool> AuthorizeAsync(string permissionSystemName, User user);

        void DeletePermission(Permission permission);

        void UninstallPermissions(IPermissionProvider permissionProvider);

        Task<IList<Permission>> GetAllPermissions();

        Task<IList<Permission>> GetPermissionByRoleId(int roleId);

        Permission GetPermissionBySystemName(string systemName);

        Task InstallPermissionsAsync(IPermissionProvider permissionProvider);

        void InsertPermission(Permission permission);

        void UpdatePermission(Permission permission);
    }
}