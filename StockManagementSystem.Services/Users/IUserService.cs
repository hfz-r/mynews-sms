﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Users
{
    public interface IUserService
    {
        Task<int> DeleteGuestUsers(DateTime? createdFromUtc, DateTime? createdToUtc);
        Task DeleteRoleAsync(Role role);
        Task DeleteUserAsync(User user);
        UserPassword GetCurrentPassword(int userId);
        Task<Role> GetRoleByIdAsync(int roleId);
        Role GetRoleBySystemName(string systemName);
        IList<Role> GetRoles(bool showHidden = false);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByGuidAsync(Guid userGuid);
        List<User> GetUsers();
        void DeleteUser(List<User> users);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserBySystemNameAsync(string systemName);
        Task<User> GetUserByUsernameAsync(string username);
        Task<string> GetUserFullNameAsync(User user);

        IList<UserPassword> GetUserPasswords(int? userId = null, PasswordFormat? passwordFormat = null,
            int? passwordsToReturn = null);

        Task<IPagedList<User>> GetUsersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            DateTime? lastLoginFrom = null, DateTime? lastLoginTo = null,
            int[] roleIds = null, string email = null, string username = null, string firstName = null,
            string lastName = null, int dayOfBirth = 0, int monthOfBirth = 0, string phone = null,
            string ipAddress = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        Task<IList<User>> GetUsersByIdsAsync(int[] userIds);
        Task<User> InsertGuestUserAsync();
        Task InsertRoleAsync(Role role);
        Task InsertUserAsync(User user);
        void InsertUserPassword(UserPassword userPassword);
        Task<bool> IsPasswordRecoveryLinkExpired(User user);
        Task<bool> IsPasswordRecoveryTokenValidAsync(User user, string token);
        bool PasswordIsExpired(User user);
        Task UpdateRoleAsync(Role role);
        Task UpdateUserAsync(User user);
        void UpdateUserPassword(UserPassword userPassword);

        #region Synchronous wrapper

        User GetUserBySystemName(string systemName);

        User GetUserByGuid(Guid userGuid);

        string GetUserFullName(User user);

        User InsertGuestUser();

        #endregion
    }
}