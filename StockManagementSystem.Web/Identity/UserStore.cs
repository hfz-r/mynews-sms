using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Web.Identity
{
    public class UserStore :
        IUserPasswordStore<User>,
        IUserEmailStore<User>,
        IUserLockoutStore<User>,
        IUserRoleStore<User>,
        IUserSecurityStampStore<User>,
        IUserLoginStore<User>,
        IUserClaimStore<User>,
        IUserAuthenticationTokenStore<User>,
        IQueryableUserStore<User>
    {
        private bool _disposed;

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IRepository<UserToken> _userTokenRepository;
        private readonly IRepository<UserClaim> _userClaimRepository;

        #region ctor

        public UserStore(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<UserLogin> userLoginRepository,
            IRepository<UserToken> userTokenRepository,
            IRepository<UserClaim> userClaimRepository)
        {
            this._userRepository = userRepository;
            this._roleRepository = roleRepository;
            this._userRoleRepository = userRoleRepository;
            this._userLoginRepository = userLoginRepository;
            this._userTokenRepository = userTokenRepository;
            this._userClaimRepository = userClaimRepository;
        }

        #endregion

        public IQueryable<User> Users => _userRepository.Table;

        #region Username/Password

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                await _userRepository.InsertAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
            }
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                await _userRepository.DeleteAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
            }
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            if (!int.TryParse(userId, out int id))
                throw new ArgumentOutOfRangeException(nameof(userId), $"{nameof(userId)} is not a valid Integer");

            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            var userEntity =
                await _userRepository.Table.FirstOrDefaultAsync(user => user.NormalizedUserName == normalizedUserName,
                    cancellationToken);
            return userEntity;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedUserName,
            CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedUserName = normalizedUserName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                await _userRepository.UpdateAsync(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
            }
        }

        #endregion

        #region Password

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        #endregion

        #region Email

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(nameof(GetEmailConfirmedAsync));
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(nameof(SetEmailConfirmedAsync));
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                throw new ArgumentNullException(nameof(normalizedEmail));

            var userEntity =
                await _userRepository.Table.FirstOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail,
                    cancellationToken);
            return userEntity;
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        #endregion

        #region SecurityStamp

        public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #region Lockout

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(++user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        #endregion

        #region UserRole

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            var role = await _roleRepository.Table.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
            await _userRoleRepository.InsertAsync(new UserRole {User = user, Role = role});
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            var roleEntity = _roleRepository.Table;
            var query = await _userRoleRepository.Table
                .Join(roleEntity, userRole => userRole.RoleId, role => role.Id,
                    (ur, r) => new {UserRole = ur, Role = r})
                .Where(x => x.Role.Name == roleName)
                .Select(ur => ur.UserRole)
                .FirstOrDefaultAsync(cancellationToken);

            await _userRoleRepository.DeleteAsync(query);
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var roleEntity = _roleRepository.Table;
            var query = _userRoleRepository.Table
                .Join(roleEntity, ur => ur.RoleId, r => r.Id, (ur, r) => new {UserRole = ur, Role = r})
                .Where(x => x.UserRole.UserId == user.Id)
                .Select(r => r.Role);

            IList<string> result = await query.Select(name => name.Name).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            var roleEntity = _roleRepository.Table;
            var query = _userRoleRepository.Table
                .Join(roleEntity, ur => ur.RoleId, r => r.Id, (ur, r) => new {UserRole = ur, Role = r})
                .Where(x => x.UserRole.UserId == user.Id)
                .Select(r => r.Role);

            var result = await query.AnyAsync(rolename => rolename.Name == roleName, cancellationToken);
            return result;
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            var userEntity = _userRepository.Table;
            var roleEntity = _roleRepository.Table;
            var query = _userRoleRepository.Table
                .Join(userEntity, ur => ur.UserId, u => u.Id, (ur, u) => new {UserRole = ur, User = u})
                .Join(roleEntity, ur => ur.UserRole.RoleId, r => r.Id, (ur, r) => new {UserRole = ur, Role = r})
                .Where(x => x.Role.Name == roleName);

            IList<User> result = await query.Select(user => user.UserRole.User).ToListAsync(cancellationToken);
            return result;
        }

        #endregion

        #region UserLogin

        public async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            if (string.IsNullOrWhiteSpace(login.LoginProvider))
                throw new ArgumentNullException(nameof(login.LoginProvider));

            if (string.IsNullOrWhiteSpace(login.ProviderKey))
                throw new ArgumentNullException(nameof(login.ProviderKey));

            var loginEntity = new UserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName,
                UserId = user.Id
            };

            await _userLoginRepository.InsertAsync(loginEntity);
        }

        public async Task RemoveLoginAsync(User user, string loginProvider, string providerKey,
            CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentNullException(nameof(providerKey));

            var loginEntity = _userLoginRepository.Table.FirstOrDefault(login =>
                login.LoginProvider == loginProvider && login.ProviderKey == providerKey);

            await _userLoginRepository.DeleteAsync(loginEntity);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var loginEntity = _userLoginRepository.Table.Where(login => login.UserId == user.Id);

            IList<UserLoginInfo> result = await loginEntity.Select(login =>
                    new UserLoginInfo(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName))
                .ToListAsync(cancellationToken);
            return result;
        }

        public async Task<User> FindByLoginAsync(string loginProvider, string providerKey,
            CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentNullException(nameof(providerKey));

            var loginEntity = await _userLoginRepository.Table.FirstOrDefaultAsync(login =>
                login.LoginProvider == loginProvider && login.ProviderKey == providerKey, cancellationToken);
            var userEntity = await _userRepository.GetByIdAsync(loginEntity.UserId);

            return userEntity;
        }

        #endregion

        #region UserToken

        public async Task SetTokenAsync(User user, string loginProvider, string name, string value,
            CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var tokenEntity = new UserToken
            {
                LoginProvider = loginProvider,
                Name = name,
                Value = value,
                UserId = user.Id
            };

            await _userTokenRepository.InsertAsync(tokenEntity);
        }

        public async Task RemoveTokenAsync(User user, string loginProvider, string name,
            CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var tokenEntity = await _userTokenRepository.Table.FirstOrDefaultAsync(token =>
                    token.UserId == user.Id && token.LoginProvider == loginProvider && token.Name == name,
                cancellationToken);

            await _userTokenRepository.DeleteAsync(tokenEntity);
        }

        public async Task<string> GetTokenAsync(User user, string loginProvider, string name,
            CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var tokenEntity = await _userTokenRepository.Table.FirstOrDefaultAsync(token =>
                    token.UserId == user.Id && token.LoginProvider == loginProvider && token.Name == name,
                cancellationToken);
            return tokenEntity.Value;
        }

        #endregion

        #region UserClaim

        public async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var claimEntity = _userClaimRepository.Table.Where(claim => claim.UserId == user.Id);

            IList<Claim> result = await claimEntity.Select(claim => new Claim(claim.ClaimType, claim.ClaimValue))
                .ToListAsync(cancellationToken);
            return result;
        }

        public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            claims.ToList().ForEach(async claim =>
            {
                try
                {
                    await _userClaimRepository.InsertAsync(new UserClaim
                    {
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value,
                        UserId = user.Id,
                    });
                }
                catch (Exception ex)
                {
                    IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
                }
            });

            return Task.CompletedTask;
        }

        public async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            var claimEntity = await _userClaimRepository.Table.FirstOrDefaultAsync(c =>
                c.ClaimType == claim.Type && c.ClaimValue == claim.Value && c.UserId == user.Id, cancellationToken);
            if (claimEntity != null)
            {
                claimEntity.ClaimType = newClaim.Type;
                claimEntity.ClaimValue = newClaim.Value;
                await _userClaimRepository.UpdateAsync(claimEntity);
            }
        }

        public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            claims.ToList().ForEach(async claim =>
            {
                try
                {
                    var claimEntity = await _userClaimRepository.Table.FirstOrDefaultAsync(c =>
                            c.ClaimType == claim.Type && c.ClaimValue == claim.Value && c.UserId == user.Id,
                        cancellationToken);
                    await _userClaimRepository.DeleteAsync(claimEntity);
                }
                catch (Exception ex)
                {
                    IdentityResult.Failed(new IdentityError {Code = ex.Message, Description = ex.Message});
                }
            });

            return Task.CompletedTask;
        }

        public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            var claimEntity =
                _userClaimRepository.Table.Where(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);

            IList<User> result = await claimEntity.Select(c => c.User).ToListAsync(cancellationToken);
            return result;
        }

        #endregion

        ~UserStore()
        {
            Dispose(false);
        }

        #region IDisposable 

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}