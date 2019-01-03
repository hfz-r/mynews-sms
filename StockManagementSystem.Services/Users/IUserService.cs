using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Services.Users
{
    public interface IUserService
    {
        Task<IPagedList<User>> GetUsersAsync(
            DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null,
            int[] roleIds = null,
            string email = null,
            string username = null,
            string name = null,
            string ipAddress = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false);


        Task<User> GetUserByGuidAsync(Guid userGuid);

        Task<User> GetUserByUsernameAsync(string userName);

        void UpdateUser(User user);

        #region Identity

        Task<User> GetUserByIdAsync(int userId);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(User user);

        Task SetEmail(User user, string newEmail);

        Task SetUsername(User user, string newUsername);

        Task<IdentityResult> ChangePassword(User user, string requestPassword);

        #region UserRoles

        Task AddUserRoles(User user, string[] roles);

        Task RemoveUserRole(User user, string role);

        #endregion

        #endregion
    }
}