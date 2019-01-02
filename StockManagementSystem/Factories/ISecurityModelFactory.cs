using System.Threading.Tasks;
using StockManagementSystem.Models.Security;

namespace StockManagementSystem.Factories
{
    public interface ISecurityModelFactory
    {
        Task<PermissionRolesModel> PreparePermissionRolesModel(PermissionRolesModel model);
    }
}