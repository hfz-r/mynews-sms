using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Models.Users;

namespace StockManagementSystem.Factories
{
    public interface IUserModelFactory
    {
        Task<UserSearchModel> PrepareUserSearchModel(UserSearchModel searchModel);

        Task<UserListModel> PrepareUserListModel(UserSearchModel searchModel);

        Task<UserModel> PrepareUserModel(UserModel model, User user);

        UserActivityLogSearchModel PrepareUserActivityLogSearchModel(UserActivityLogSearchModel searchModel, User user);

        Task<UserActivityLogListModel> PrepareUserActivityLogListModel(UserActivityLogSearchModel searchModel, User user);
    }
}