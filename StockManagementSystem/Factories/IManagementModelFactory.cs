using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Models.Management;

namespace StockManagementSystem.Factories
{
    public interface IManagementModelFactory
    {
        Task<AssignUserModel> PrepareAssignUserListbyStoreModel(int storeID);
        Task<AssignUserModel> PrepareAssignUserListModel();
        Task<AssignUserListModel> PrepareAssignUserListModel(AssignUserSearchModel searchModel);
        Task<AssignUserModel> PrepareAssignUserModel(AssignUserModel model, StoreUserAssign storeUserAssign);
        Task<AssignUserSearchModel> PrepareAssignUserSearchModel(AssignUserSearchModel searchModel);
        Task<GroupOutletModel> PrepareGroupOutletListModel();
        Task<GroupOutletListModel> PrepareGroupOutletListModel(GroupOutletSearchModel searchModel);
        Task<GroupOutletModel> PrepareGroupOutletModel(GroupOutletModel model, StoreGrouping storeGrouping);
        Task<GroupOutletSearchModel> PrepareGroupOutletSearchModel(GroupOutletSearchModel searchModel);
        Task<OutletManagementContainerModel> PrepareOutletManagementContainerModel(OutletManagementContainerModel outletManagementContainerModel);
    }
}