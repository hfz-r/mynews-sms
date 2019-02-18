using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<User> GetAuthenticatedUserAsync();
        Task SignInAsync(User user, bool isPersistent);
        Task SignOutAsync();
    }
}