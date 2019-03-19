using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;

namespace StockManagementSystem.Factories
{
    public interface IUserAccountModelFactory
    {
        Task<LoginViewModel> PrepareLoginModel();

        Task<RegisterViewModel> PrepareRegisterModel(RegisterViewModel model, bool excludeProperties);

        Task<ForgotPasswordViewModel> PrepareForgotPasswordModel();

        Task<ForgotPasswordConfirmViewModel> PrepareForgotPasswordConfirmModel();

        Task<ChangePasswordModel> PrepareChangePasswordModel();

        Task<UserNavigationModel> PrepareUserNavigationModel(int selectedTabId = 0);

        Task<UserInfoModel> PrepareUserInfoModel(UserInfoModel model, User user, bool excludeProperties);

        Task<UserAvatarModel> PrepareUserAvatarModel(UserAvatarModel model);
    }
}