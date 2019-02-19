using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Models.Management;

namespace StockManagementSystem.Factories
{
    public interface IManagementModelFactory
    {
        Task<AssignUserModel> PrepareAssignUserListModel();
        Task<AssignUserListModel> PrepareAssignUserListModel(AssignUserSearchModel searchModel);
        Task<AssignUserModel> PrepareAssignUserModel(AssignUserModel model, StoreUserAssign storeUserAssign);
        Task<GroupOutletModel> PrepareGroupOutletListModel();
        Task<GroupOutletListModel> PrepareGroupOutletListModel(GroupOutletSearchModel searchModel);
        Task<GroupOutletModel> PrepareGroupOutletModel(GroupOutletModel model, StoreGrouping storeGrouping);
        Task<OutletManagementContainerModel> PrepareOutletManagementContainerModel(OutletManagementContainerModel outletManagementContainerModel);
    }
}