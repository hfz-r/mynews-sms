using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRepository<Role> _roleRepository;

        public RoleService(
            ICacheManager cacheManager,
            IStaticCacheManager staticCacheManager,
            RoleManager<Role> roleManager,
            IRepository<Role> roleRepository)
        {
            _cacheManager = cacheManager;
            _staticCacheManager = staticCacheManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
        }

        public async Task<Role> GetRoleBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var key = string.Format(RoleDefaults.GetRoleBySystemName, systemName);
            return await _cacheManager.Get(key, async () =>
            {
                var role = await _roleRepository.Table.FirstOrDefaultAsync(r => r.SystemName == systemName);
                return role;
            });
        }

        #region Identity

        public async Task<IList<Role>> GetRolesAsync()
        {
            var key = string.Format(RoleDefaults.GetRolesKey);
            return await _cacheManager.Get(key, async () =>
            {
                var roles = await _roleManager.Roles.ToListAsync();
                return roles;
            });
        }

        public async Task<Role> GetRoleByIdAsync(int roleId)
        {
            if (roleId == 0)
                return null;

            var key = string.Format(RoleDefaults.GetRoleByIdKey, roleId);
            return await _cacheManager.Get(key, async () =>
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                return role;
            });
        }

        public async Task InsertRoleAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            await _roleManager.CreateAsync(role);

            _cacheManager.RemoveByPattern(RoleDefaults.RolesPatternCacheKey);
            _staticCacheManager.RemoveByPattern(RoleDefaults.RolesPatternCacheKey);
        }

        public async Task UpdateRoleAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            await _roleManager.UpdateAsync(role);

            _cacheManager.RemoveByPattern(RoleDefaults.RolesPatternCacheKey);
            _staticCacheManager.RemoveByPattern(RoleDefaults.RolesPatternCacheKey);
        }

        public async Task DeleteRoleAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            await _roleManager.DeleteAsync(role);

            _cacheManager.RemoveByPattern(RoleDefaults.RolesPatternCacheKey);
            _staticCacheManager.RemoveByPattern(RoleDefaults.RolesPatternCacheKey);
        }

        #endregion
    }
}