using System.Threading.Tasks;
using StockManagementSystem.Models.Roles;

namespace StockManagementSystem.Factories
{
    public interface IRoleModelFactory
    {
        Task<RoleSearchModel> PrepareRoleSearchModelAsync(RoleSearchModel searchModel);

        Task<RoleListModel> PrepareRoleListModelAync(RoleSearchModel searchModel);
    }
}