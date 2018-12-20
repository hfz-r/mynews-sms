using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Services.Roles
{
    public interface IRoleService
    {
        Task<IList<Role>> GetRoles();
    }
}