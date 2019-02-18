using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Services.Security
{
    public class PermissionService : IPermissionService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<PermissionRoles> _permissionRolesRepository;

        public PermissionService(
            ICacheManager cacheManager,
            IUserService userService,
            IWorkContext workContext,
            IStaticCacheManager staticCacheManager,
            IRepository<Permission> permissionRepository,
            IRepository<PermissionRoles> permissionRolesRepository)
        {
            _cacheManager = cacheManager;
            _userService = userService;
            _workContext = workContext;
            _staticCacheManager = staticCacheManager;
            _permissionRepository = permissionRepository;
            _permissionRolesRepository = permissionRolesRepository;
        }

        /// <summary>
        /// Get permission records by role id
        /// </summary>
        protected async Task<IList<Permission>> GetPermissionByRoleId(int roleId)
        {
            var key = string.Format(SecurityDefaults.GetPermissionByRoleIdKey, roleId);
            return await _cacheManager.Get(key, async () =>
            {
                var permissions = _permissionRepository.Table
                    .Join(_permissionRolesRepository.Table, p => p.Id, pr => pr.PermissionId,
                        (p, pr) => new {Permission = p, PermissionRoles = pr})
                    .Where(pr => pr.PermissionRoles.RoleId == roleId)
                    .OrderBy(p => p.Permission.Id)
                    .Select(p => p.Permission);

                return await permissions.ToListAsync();
            });
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        protected async Task<bool> AuthorizeAsync(string permissionSystemName, int roleId)
        {
            if (string.IsNullOrEmpty(permissionSystemName))
                return false;

            var key = string.Format(SecurityDefaults.PermissionsAllowedKey, roleId, permissionSystemName);
            return await _staticCacheManager.Get(key, async () =>
            {
                var permissions = await GetPermissionByRoleId(roleId);
                foreach (var permission in permissions)
                {
                    if (permission.SystemName.Equals(permissionSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Delete a permission
        /// </summary>
        public virtual void DeletePermission(Permission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            _permissionRepository.Delete(permission);

            _cacheManager.RemoveByPattern(SecurityDefaults.PermissionsPatternCacheKey);
            _staticCacheManager.RemoveByPattern(SecurityDefaults.PermissionsPatternCacheKey);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        public virtual Permission GetPermissionBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var permission = _permissionRepository.Table.FirstOrDefault(p => p.SystemName == systemName);
            return permission;
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        public async Task<IList<Permission>> GetAllPermissions()
        {
            return await _permissionRepository.Table.OrderBy(p => p.Name).ToListAsync();
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        public virtual void InsertPermission(Permission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            _permissionRepository.Insert(permission);

            _cacheManager.RemoveByPattern(SecurityDefaults.PermissionsPatternCacheKey);
            _staticCacheManager.RemoveByPattern(SecurityDefaults.PermissionsPatternCacheKey);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        public virtual void UpdatePermission(Permission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            _permissionRepository.Update(permission);

            _cacheManager.RemoveByPattern(SecurityDefaults.PermissionsPatternCacheKey);
            _staticCacheManager.RemoveByPattern(SecurityDefaults.PermissionsPatternCacheKey);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        public async Task InstallPermissionsAsync(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            var defaultPermissions = permissionProvider.GetDefaultPermissions();

            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionBySystemName(permission.SystemName);
                if (permission1 != null)
                    continue;

                //new permission (install it)
                permission1 = new Permission
                {
                    Name = permission.Name,
                    SystemName = permission.SystemName,
                    Category = permission.Category,
                };

                foreach (var defaultPermission in defaultPermissions)
                {
                    var role = _userService.GetRoleBySystemName(defaultPermission.RoleSystemName);
                    if (role == null)
                    {
                        //new role (save it)
                        role = new Role
                        {
                            Name = defaultPermission.RoleSystemName,
                            Active = true,
                            SystemName = defaultPermission.RoleSystemName,
                        };
                        await _userService.InsertRoleAsync(role);
                    }

                    var defaultMappingProvided = defaultPermission.Permissions.Any(dp => dp.SystemName == permission1.SystemName);
                    var mappingExists = role.PermissionRoles.Any(pr => pr.Permission.SystemName == permission1.SystemName);

                    if (defaultMappingProvided && !mappingExists)
                    {
                        permission1.PermissionRoles.Add(new PermissionRoles {Role = role});
                    }
                }

                InsertPermission(permission1);
            }
        }

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        public virtual void UninstallPermissions(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var p = GetPermissionBySystemName(permission.SystemName);
                if (p == null)
                    continue;

                DeletePermission(p);
            }
        }

        #region Authorize permission

        public async Task<bool> AuthorizeAsync(Permission permission)
        {
            return await AuthorizeAsync(permission, _workContext.CurrentUser);
        }

        public async Task<bool> AuthorizeAsync(Permission permission, User user)
        {
            if (permission == null)
                return false;

            if (user == null)
                return false;

            return await AuthorizeAsync(permission.SystemName, user);
        }

        public async Task<bool> AuthorizeAsync(string permissionSystemName)
        {
            return await AuthorizeAsync(permissionSystemName, _workContext.CurrentUser);
        }

        public async Task<bool> AuthorizeAsync(string permissionSystemName, User user)
        {
            if (string.IsNullOrEmpty(permissionSystemName))
                return false;

            var roles = user.Roles.Where(r => r.Active);
            foreach (var role in roles)
                if (await AuthorizeAsync(permissionSystemName, role.Id))
                    return true;

            return false;
        }

        #endregion
    }
}