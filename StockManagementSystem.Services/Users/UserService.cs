﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Services.Users
{
    public class UserService : IUserService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;

        public UserService(
            ICacheManager cacheManager,
            IStaticCacheManager staticCacheManager,
            UserManager<User> userManager,
            IRepository<UserRole> userRoleRepository,
            IRepository<User> userRepository)
        {
            _cacheManager = cacheManager;
            _staticCacheManager = staticCacheManager;
            _userManager = userManager;
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByUsernameAsync(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            var user = await _userRepository.Table.FirstOrDefaultAsync(u => u.UserName == userName);
            return user;
        }

        public Task<IPagedList<User>> GetUsersAsync(
            DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null,
            int[] roleIds = null,
            string email = null,
            string username = null,
            string name = null,
            string ipAddress = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false)
        {
            var query = _userRepository.Table;

            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);

            if (roleIds != null && roleIds.Length > 0)
            {
                query = query.Join(_userRoleRepository.Table, x => x.Id, y => y.UserId,
                        (x, y) => new {User = x, UserRole = y})
                    .Where(z => roleIds.Contains(z.UserRole.RoleId))
                    .Select(z => z.User)
                    .Distinct();
            }

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => u.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(u => u.UserName.Contains(username));
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(u => u.Name.Contains(name));
            if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
                query = query.Where(u => u.LastIpAddress == ipAddress);

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<IPagedList<User>>(new PagedList<User>(query, pageIndex, pageSize,
                getOnlyTotalCount));
        }

        public virtual void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _userRepository.Update(user);

            _cacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
            _staticCacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
        }

        public async Task<User> GetUserByGuidAsync(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return null;

            var user = await _userRepository.Table.FirstOrDefaultAsync(c => c.UserGuid == userGuid);
            return user;
        }

        #region Identity 

        public async Task<User> GetUserByIdAsync(int userId)
        {
            if (userId == 0)
                return null;

            var key = string.Format(UserDefaults.GetUserByIdKey, userId);
            return await _cacheManager.Get(key, async () =>
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                return user;
            });
        }

        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _userManager.UpdateAsync(user);

            _cacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
            _staticCacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
        }

        public async Task DeleteUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _userManager.DeleteAsync(user);

            _cacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
            _staticCacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
        }

        public async Task SetEmail(User user, string newEmail)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (newEmail == null)
                throw new Exception("Email cannot be null");

            newEmail = newEmail.Trim();
            var oldEmail = user.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new Exception("New email is not valid");

            var user2 = await _userManager.FindByEmailAsync(newEmail);
            if (user2 != null && user.Id != user2.Id)
                throw new Exception("The e-mail address is already in use");

            user.Email = newEmail;
            await _userManager.UpdateAsync(user);

            _cacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
            _staticCacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);

            if (string.IsNullOrEmpty(oldEmail) ||
                oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase))
                return;
        }

        public async Task SetUsername(User user, string newUsername)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            newUsername = newUsername.Trim();

            var user2 = await GetUserByUsernameAsync(newUsername);
            if (user2 != null && user.Id != user2.Id)
                throw new Exception("The username is already in use");

            user.UserName = newUsername;

            await _userManager.UpdateAsync(user);

            _cacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
            _staticCacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
        }

        public async Task<IdentityResult> ChangePassword(User user, string requestPassword)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(requestPassword))
                throw new ArgumentNullException(nameof(requestPassword));

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, requestPassword);

            return await _userManager.UpdateAsync(user);
        }

        #region UserRoles

        public async Task AddUserRoles(User user, string[] roles)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (roles.Length == 0)
                return;

            await _userManager.AddToRolesAsync(user, roles);

            _cacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
            _staticCacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
        }

        public async Task RemoveUserRole(User user, string role)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            await _userManager.RemoveFromRoleAsync(user, role);

            _cacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
            _staticCacheManager.RemoveByPattern(UserDefaults.UsersPatternCacheKey);
        }

        #endregion

        #endregion
    }
}