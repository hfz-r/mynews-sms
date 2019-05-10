using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Services
{
    public interface IRoleApiService
    {
        int GetRolesCount(bool active = true);

        Role GetRoleById(int id);

        IList<Role> GetRoles(IList<int> ids = null, int limit = 50, int page = 1, int sinceId = 0, IList<int> permissionIds = null);
    }
}