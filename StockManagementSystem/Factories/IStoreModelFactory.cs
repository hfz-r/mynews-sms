using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Models.Stores;

namespace StockManagementSystem.Factories
{
    public interface IStoreModelFactory
    {
        Task<StoreSearchModel> PrepareStoreSearchModel(StoreSearchModel searchModel);

        Task<StoreListModel> PrepareStoreListModel(StoreSearchModel searchModel);

        Task<StoreModel> PrepareStoreModel(StoreModel model, Store store, bool excludeProperties = false);

        Task<UserStoreListModel> PrepareUserStoreListModel(UserStoreSearchModel searchModel, Store store);

        Task<AddUserToStoreSearchModel> PrepareAddUserToStoreSearchModel(AddUserToStoreSearchModel searchModel);

        Task<AddUserToStoreListModel> PrepareAddUserToStoreListModel(AddUserToStoreSearchModel searchModel);
    }
}