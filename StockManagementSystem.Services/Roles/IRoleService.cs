using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Services.Roles
{
    public interface IRoleService
    {
        Task<IList<Role>> GetRolesAsync();

        Task<Role> GetRoleByIdAsync(int roleId);

        Task InsertRoleAsync(Role role);

        Task UpdateRoleAsync(Role role);

        Task DeleteRoleAsync(Role role);

        Task<Role> GetRoleBySystemNameAsync(string systemName);
    }
}