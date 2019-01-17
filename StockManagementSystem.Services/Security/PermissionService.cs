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
using StockManagementSystem.Services.Roles;

namespace StockManagementSystem.Services.Security
{
    public class PermissionService : IPermissionService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRoleService _roleService;
        private readonly IWorkContext _workContext;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<PermissionRoles> _permissionRolesRepository;

        public PermissionService(
            ICacheManager cacheManager,
            IStaticCacheManager staticCacheManager,
            IRoleService roleService,
            IWorkContext workContext,
            IRepository<Permission> permissionRepository,
            IRepository<PermissionRoles> permissionRolesRepository)
        {
            _cacheManager = cacheManager;
            _staticCacheManager = staticCacheManager;
            _roleService = roleService;
            _workContext = workContext;
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

            foreach (var p in permissions)
            {
                var permission = GetPermissionBySystemName(p.SystemName);
                if (permission != null)
                    continue;

                permission = new Permission
                {
                    Name = p.Name,
                    SystemName = p.SystemName,
                    Category = p.Category,
                };

                foreach (var defaultPermission in defaultPermissions)
                {
                    var role = await _roleService.GetRoleBySystemNameAsync(defaultPermission.RoleSystemName);
                    if (role == null)
                    {
                        //new role (save it)
                        role = new Role
                        {
                            Name = defaultPermission.RoleSystemName,
                            SystemName = defaultPermission.RoleSystemName,
                        };
                        await _roleService.InsertRoleAsync(role);
                    }

                    var defaultMappingProvided =
                        defaultPermission.Permissions.Any(dp => dp.SystemName == permission.SystemName);
                    var mappingExists =
                        role.PermissionRoles.Any(pr => pr.Permission.SystemName == permission.SystemName);

                    if (defaultMappingProvided && !mappingExists)
                    {
                        permission.PermissionRoles.Add(new PermissionRoles {Role = role});
                    }
                }

                InsertPermission(permission);
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

            var roles = user.UserRoles.Select(r => r.Role).ToList();
            foreach (var role in roles)
                if (await AuthorizeAsync(permissionSystemName, role.Id))
                    return true;

            return false;
        }

        #endregion
    }
}