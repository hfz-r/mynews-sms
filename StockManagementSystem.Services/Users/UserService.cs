using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Data.Extensions;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Data;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Events;

namespace StockManagementSystem.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserSettings _userSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<UserPassword> _userPasswordRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly string _entityName;

        public UserService(
            UserSettings userSettings,
            ICacheManager cacheManager,
            IDataProvider dataProvider,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<UserPassword> userPasswordRepository,
            IRepository<GenericAttribute> gaRepository,
            IStaticCacheManager staticCacheManager)
        {
            _userSettings = userSettings;
            _cacheManager = cacheManager;
            _dataProvider = dataProvider;
            _dbContext = dbContext;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userPasswordRepository = userPasswordRepository;
            _gaRepository = gaRepository;
            _staticCacheManager = staticCacheManager;
            _entityName = typeof(User).Name;
        }

        #region Users

        /// <summary>
        /// Gets all users
        /// </summary>
        public Task<IPagedList<User>> GetUsersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null, DateTime? lastLoginFrom = null, DateTime? lastLoginTo = null,
           int[] roleIds = null, string email = null, string username = null, string firstName = null, string lastName = null,
           int dayOfBirth = 0, int monthOfBirth = 0, string phone = null, string ipAddress = null,
           int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _userRepository.Table;

            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
            query = query.Where(c => !c.Deleted);

            if (roleIds != null && roleIds.Length > 0)
            {
                query = query.Join(_userRoleRepository.Table, x => x.Id, y => y.UserId,
                        (x, y) => new { User = x, UserRole = y })
                    .Where(z => roleIds.Contains(z.UserRole.RoleId))
                    .Select(z => z.User)
                    .Distinct();
            }

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => u.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(u => u.Username.Contains(username));

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == _entityName &&
                                z.Attribute.Key == UserDefaults.FirstNameAttribute &&
                                z.Attribute.Value.Contains(firstName))
                    .Select(z => z.Customer);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == _entityName &&
                                z.Attribute.Key == UserDefaults.LastNameAttribute &&
                                z.Attribute.Value.Contains(lastName))
                    .Select(z => z.Customer);
            }

            //date of birth is stored as a string into database with following format YYYY-MM-DD (for example, 1983-02-18).
            if (dayOfBirth > 0 && monthOfBirth > 0)
            {
                //both are specified
                var dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" +
                                     dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

                //z.Attribute.Value.Length - dateOfBirthStr.Length = 5
                //dateOfBirthStr.Length = 5
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == _entityName &&
                                z.Attribute.Key == UserDefaults.DateOfBirthAttribute &&
                                z.Attribute.Value.Substring(5, 5) == dateOfBirthStr)
                    .Select(z => z.Customer);
            }
            else if (dayOfBirth > 0)
            {
                //only day is specified
                var dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

                //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
                //dateOfBirthStr.Length = 2
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == _entityName &&
                                z.Attribute.Key == UserDefaults.DateOfBirthAttribute &&
                                z.Attribute.Value.Substring(8, 2) == dateOfBirthStr)
                    .Select(z => z.Customer);
            }
            else if (monthOfBirth > 0)
            {
                //only month is specified
                var dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == _entityName &&
                                z.Attribute.Key == UserDefaults.DateOfBirthAttribute &&
                                z.Attribute.Value.Contains(dateOfBirthStr))
                    .Select(z => z.Customer);
            }
            //search by phone
            if (!string.IsNullOrWhiteSpace(phone))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == _entityName &&
                                z.Attribute.Key == UserDefaults.PhoneAttribute &&
                                z.Attribute.Value.Contains(phone))
                    .Select(z => z.Customer);
            }

            //search by IpAddress
            if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
            {
                query = query.Where(w => w.LastIpAddress == ipAddress);
            }

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<IPagedList<User>>(new PagedList<User>(query, pageIndex, pageSize, getOnlyTotalCount));
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        public async Task DeleteUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (user.IsSystemAccount)
                throw new DefaultException($"System user account ({user.SystemName}) could not be deleted");

            user.Deleted = true;

            await UpdateUserAsync(user);

            _eventPublisher.EntityDeleted(user);
        }

        /// <summary>
        /// Gets a user
        /// </summary>
        public async Task<User> GetUserByIdAsync(int userId)
        {
            if (userId == 0)
                return null;

            return await _userRepository.GetByIdAsync(userId);
        }

        /// <summary>
        /// Get users by identifiers
        /// </summary>
        public async Task<IList<User>> GetUsersByIdsAsync(int[] userIds)
        {
            if (userIds == null || userIds.Length == 0)
                return new List<User>();

            var query = from u in _userRepository.Table
                        where userIds.Contains(u.Id) && !u.Deleted
                        select u;
            var users = await query.ToListAsync();

            //sort by passed identifiers
            var sortedUsers = new List<User>();
            foreach (var id in userIds)
            {
                var user = users.Find(x => x.Id == id);
                if (user != null)
                    sortedUsers.Add(user);
            }

            return sortedUsers;
        }

        /// <summary>
        /// Gets a user by GUID
        /// </summary>
        public async Task<User> GetUserByGuidAsync(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return null;

            var user = await _userRepository.Table
                .FirstOrDefaultAsync(c => c.UserGuid == userGuid);

            return user;
        }

        public List<User> GetUsers()
        {
            var user = _userRepository.Table.ToList();
            return user;
        }

        public virtual void DeleteUser(List<User> users)
        {
            if (users == null)
                throw new ArgumentNullException(nameof(users));

            _userRepository.Delete(users);
        }
        
        /// <summary>
        /// Gets a user by email
        /// </summary>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var user = await _userRepository.Table.FirstOrDefaultAsync(e => e.Email == email);

            return user;
        }

        /// <summary>
        /// Gets a user by system name
        /// </summary>
        public async Task<User> GetUserBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var user = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.SystemName == systemName);

            return user;
        }

        /// <summary>
        /// Gets a user by username
        /// </summary>
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var user = await _userRepository.Table
                .FirstOrDefaultAsync(u => u.Username == username);

            return user;
        }

        /// <summary>
        /// Insert a user
        /// </summary>
        public async Task InsertUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _userRepository.InsertAsync(user);

            _eventPublisher.EntityInserted(user);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _userRepository.UpdateAsync(user);

            _eventPublisher.EntityUpdated(user);
        }

        /// <summary>
        /// Get full name
        /// </summary>
        public async Task<string> GetUserFullNameAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var firstName = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.FirstNameAttribute);
            var lastName = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.LastNameAttribute);

            var fullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                fullName = $"{firstName} {lastName}";
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!string.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }

            return fullName;
        }

        #endregion

        #region Roles

        /// <summary>
        /// Delete a role
        /// </summary>
        public async Task DeleteRoleAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (role.IsSystemRole)
                throw new DefaultException("System role could not be deleted");

            await _roleRepository.DeleteAsync(role);

            _cacheManager.RemoveByPattern(UserServiceDefaults.RolesPatternCacheKey);
        }

        /// <summary>
        /// Gets a role
        /// </summary>
        public async Task<Role> GetRoleByIdAsync(int roleId)
        {
            if (roleId == 0)
                return null;

            return await _roleRepository.GetByIdAsync(roleId);
        }

        /// <summary>
        /// Gets a role by system name
        /// </summary>
        public Role GetRoleBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var key = string.Format(UserServiceDefaults.RolesBySystemNameCacheKey, systemName);
            return _cacheManager.Get(key, () =>
            {
                var query = from r in _roleRepository.Table
                            orderby r.Id
                            where r.SystemName == systemName
                            select r;
                var role = query.FirstOrDefault();

                return role;
            });
        }

        /// <summary>
        /// Gets all roles
        /// </summary>
        public IList<Role> GetRoles(bool showHidden = false)
        {
            var key = string.Format(UserServiceDefaults.RolesAllCacheKey, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from r in _roleRepository.Table
                            orderby r.Name
                            where showHidden || r.Active
                            select r;
                var roles = query.ToList();

                return roles;
            });
        }

        /// <summary>
        /// Insert a role
        /// </summary>
        public async Task InsertRoleAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            await _roleRepository.InsertAsync(role);

            _cacheManager.RemoveByPattern(UserServiceDefaults.RolesPatternCacheKey);
        }

        /// <summary>
        /// Update a role
        /// </summary>
        public async Task UpdateRoleAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            await _roleRepository.UpdateAsync(role);

            _cacheManager.RemoveByPattern(UserServiceDefaults.RolesPatternCacheKey);
        }

        #endregion

        #region User password

        /// <summary>
        /// Gets user passwords
        /// </summary>
        public IList<UserPassword> GetUserPasswords(int? userId = null, PasswordFormat? passwordFormat = null, int? passwordsToReturn = null)
        {
            var query = _userPasswordRepository.Table;

            //filter by user
            if (userId.HasValue)
                query = query.Where(password => password.UserId == userId.Value);

            //filter by password format
            if (passwordFormat.HasValue)
                query = query.Where(password => password.PasswordFormatId == (int)passwordFormat.Value);

            //get the latest passwords
            if (passwordsToReturn.HasValue)
                query = query.OrderByDescending(password => password.CreatedOnUtc).Take(passwordsToReturn.Value);

            return query.ToList();
        }

        /// <summary>
        /// Get current user password
        /// </summary>
        public UserPassword GetCurrentPassword(int userId)
        {
            if (userId == 0)
                return null;

            //return the latest password
            return GetUserPasswords(userId, passwordsToReturn: 1).FirstOrDefault();
        }

        /// <summary>
        /// Insert a user password
        /// </summary>
        public void InsertUserPassword(UserPassword userPassword)
        {
            if (userPassword == null)
                throw new ArgumentNullException(nameof(userPassword));

            _userPasswordRepository.Insert(userPassword);
        }

        /// <summary>
        /// Update a user password
        /// </summary>
        public void UpdateUserPassword(UserPassword userPassword)
        {
            if (userPassword == null)
                throw new ArgumentNullException(nameof(userPassword));

            _userPasswordRepository.Update(userPassword);
        }

        /// <summary>
        /// Check whether password recovery token is valid
        /// </summary>
        public async Task<bool> IsPasswordRecoveryTokenValidAsync(User user, string token)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var cPrt = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.PasswordRecoveryTokenAttribute);
            if (string.IsNullOrEmpty(cPrt))
                return false;

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        /// <summary>
        /// Check whether password recovery link is expired
        /// </summary>
        public async Task<bool> IsPasswordRecoveryLinkExpired(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (_userSettings.PasswordRecoveryLinkDaysValid == 0)
                return false;

            var generatedDate = await _genericAttributeService.GetAttributeAsync<DateTime?>(user,
                UserDefaults.PasswordRecoveryTokenDateGeneratedAttribute);
            if (!generatedDate.HasValue)
                return false;

            var daysPassed = (DateTime.UtcNow - generatedDate.Value).TotalDays;
            if (daysPassed > _userSettings.PasswordRecoveryLinkDaysValid)
                return true;

            return false;
        }

        /// <summary>
        /// Check whether user password is expired 
        /// </summary>
        public bool PasswordIsExpired(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //the guests don't have a password
            if (user.IsGuest())
                return false;

            //password lifetime is disabled for user
            if (!user.Roles.Any(role => role.Active && role.EnablePasswordLifetime))
                return false;

            //setting disabled for all
            if (_userSettings.PasswordLifetime == 0)
                return false;

            //cache result between HTTP requests
            var cacheKey = string.Format(UserServiceDefaults.UserPasswordLifetimeCacheKey, user.Id);

            //get current password usage time
            var currentLifetime = _staticCacheManager.Get(cacheKey, () =>
            {
                var userPassword = this.GetCurrentPassword(user.Id);
                //password is not found, so return max value to force user to change password
                if (userPassword == null)
                    return int.MaxValue;

                return (DateTime.UtcNow - userPassword.CreatedOnUtc).Days;
            });

            return currentLifetime >= _userSettings.PasswordLifetime;
        }

        #endregion

        #region Guest

        /// <summary>
        /// Insert a guest user
        /// </summary>
        public async Task<User> InsertGuestUser()
        {
            var user = new User
            {
                UserGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            //add to 'Guests' role
            var guestRole = GetRoleBySystemName(UserDefaults.GuestsRoleName);
            if (guestRole == null)
                throw new DefaultException("'Guest' role could not be loaded");

            user.AddUserRole(new UserRole() { Role = guestRole });

            await _userRepository.InsertAsync(user);

            return user;
        }

        /// <summary>
        /// Delete guest user records
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <returns>Number of deleted users</returns>
        public async Task<int> DeleteGuestUsers(DateTime? createdFromUtc, DateTime? createdToUtc)
        {
            //parameters
            var pCreatedFromUtc = _dataProvider.GetDateTimeParameter("CreatedFromUtc", createdFromUtc);
            var pCreatedToUtc = _dataProvider.GetDateTimeParameter("CreatedToUtc", createdToUtc);
            var pTotalRecordsDeleted = _dataProvider.GetOutputInt32Parameter("TotalRecordsDeleted");

            //invoke stored procedure
            await _dbContext.ExecuteSqlCommandAsync(
                "EXEC [DeleteGuests] @CreatedFromUtc, @CreatedToUtc, @TotalRecordsDeleted OUTPUT",
                false,
                null,
                pCreatedFromUtc,
                pCreatedToUtc,
                pTotalRecordsDeleted);

            var totalRecordsDeleted = pTotalRecordsDeleted.Value != DBNull.Value ? Convert.ToInt32(pTotalRecordsDeleted.Value) : 0;
            return totalRecordsDeleted;
        }

        #endregion
    }
}