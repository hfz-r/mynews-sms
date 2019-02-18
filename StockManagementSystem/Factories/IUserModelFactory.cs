using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Users;

namespace StockManagementSystem.Factories
{
    public interface IUserModelFactory
    {
        Task<UserSearchModel> PrepareUserSearchModel(UserSearchModel searchModel);

        Task<UserListModel> PrepareUserListModel(UserSearchModel searchModel);

        Task<UserModel> PrepareUserModel(UserModel model, User user, bool excludeProperties = false);

        UserActivityLogSearchModel PrepareUserActivityLogSearchModel(UserActivityLogSearchModel searchModel, User user);

        Task<UserActivityLogListModel> PrepareUserActivityLogListModel(UserActivityLogSearchModel searchModel, User user);
    }
}