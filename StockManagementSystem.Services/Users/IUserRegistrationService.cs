using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Users
{
    public interface IUserRegistrationService
    {
        Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request);

        Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request);

        Task SetEmailAsync(User user, string newEmail);

        Task SetUsernameAsync(User user, string newUsername);

        Task<UserLoginResults> ValidateUserAsync(string usernameOrEmail, string password);
    }
}