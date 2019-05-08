using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Services
{
    public class RoleApiService : IRoleApiService
    {
        private readonly IRepository<Role> _roleRepository;

        public RoleApiService(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        #region Utilities

        private IQueryable<Role> GetRolesQuery(IList<int> ids = null, IList<int> permissionIds = null)
        {
            var query = _roleRepository.Table;

            if (ids != null && ids.Count > 0)
                query = query.Where(d => ids.Contains(d.Id));

            if (permissionIds != null && permissionIds.Count > 0)
            {
                var result = new List<Role>();

                foreach (var role in query)
                {
                    var r = role;

                    var permissionRoles = role.PermissionRoles.Where(x => permissionIds.Contains(x.PermissionId)).ToList();
                    r.PermissionRoles = permissionRoles;

                    result.Add(r);
                }

                query = result.AsQueryable();
            }

            query = query.OrderBy(d => d.Id);

            return query;
        }

        #endregion

        public IList<Role> GetRoles(
            IList<int> ids = null,
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            IList<int> permissionIds = null)
        {
            var query = GetRolesQuery(ids, permissionIds);

            if (sinceId > 0)
                query = query.Where(device => device.Id > sinceId);

            return new ApiList<Role>(query, page - 1, limit);
        }

        public int GetRolesCount(bool active = true)
        {
            return _roleRepository.Table.Count(role => role.Active == active);
        }

        public Role GetRoleById(int id)
        {
            if (id == 0)
                return null;

            var role = _roleRepository.Table.FirstOrDefault(r => r.Id == id);

            return role;
        }
    }
}