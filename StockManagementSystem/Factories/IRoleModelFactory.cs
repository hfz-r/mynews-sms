using System.Threading.Tasks;
using StockManagementSystem.Models.Roles;

namespace StockManagementSystem.Factories
{
    public interface IRoleModelFactory
    {
        Task<RoleSearchModel> PrepareRoleSearchModel(RoleSearchModel searchModel);

        Task<RoleListModel> PrepareRoleListModel(RoleSearchModel searchModel);
    }
}