using System.Threading.Tasks;
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
    }
}