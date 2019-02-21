using System;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Security;

namespace StockManagementSystem.Services.Users
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;

        public UserRegistrationService(
            UserSettings userSettings, 
            IUserService userService, 
            IEncryptionService encryptionService)
        {
            _userSettings = userSettings;
            _userService = userService;
            _encryptionService = encryptionService;
        }

        /// <summary>
        /// Check whether the entered password matches with a saved one
        /// </summary>
        protected bool PasswordsMatch(UserPassword userPassword, string enteredPassword)
        {
            if (userPassword == null || string.IsNullOrEmpty(enteredPassword))
                return false;

            var savedPassword = string.Empty;
            switch (userPassword.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    savedPassword = enteredPassword;
                    break;
                case PasswordFormat.Encrypted:
                    savedPassword = _encryptionService.EncryptText(enteredPassword);
                    break;
                case PasswordFormat.Hashed:
                    savedPassword = _encryptionService.CreatePasswordHash(enteredPassword, userPassword.PasswordSalt,
                        _userSettings.HashedPasswordFormat);
                    break;
            }

            if (userPassword.Password == null)
                return false;

            return userPassword.Password.Equals(savedPassword);
        }

        /// <summary>
        /// Validate user
        /// </summary>
        public async Task<UserLoginResults> ValidateUserAsync(string usernameOrEmail, string password)
        {
            var user = _userSettings.UsernamesEnabled
                ? await _userService.GetUserByUsernameAsync(usernameOrEmail)
                : await _userService.GetUserByEmailAsync(usernameOrEmail);

            if (user == null)
                return UserLoginResults.UserNotExist;
            if (user.Deleted)
                return UserLoginResults.Deleted;
            if (!user.Active)
                return UserLoginResults.NotActive;
            //only registered can login
            if (!user.IsRegistered())
                return UserLoginResults.NotRegistered;
            //check whether a user is locked out
            if (user.CannotLoginUntilDateUtc.HasValue && user.CannotLoginUntilDateUtc.Value > DateTime.UtcNow)
                return UserLoginResults.LockedOut;

            if (!PasswordsMatch(_userService.GetCurrentPassword(user.Id), password))
            {
                //wrong password
                user.FailedLoginAttempts++;
                if (_userSettings.FailedPasswordAllowedAttempts > 0 && user.FailedLoginAttempts >= _userSettings.FailedPasswordAllowedAttempts)
                {
                    //lock out
                    user.CannotLoginUntilDateUtc = DateTime.UtcNow.AddMinutes(_userSettings.FailedPasswordLockoutMinutes);
                    //reset the counter
                    user.FailedLoginAttempts = 0;
                }

                await _userService.UpdateUserAsync(user);

                return UserLoginResults.WrongPassword;
            }

            //update login details
            user.FailedLoginAttempts = 0;
            user.CannotLoginUntilDateUtc = null;
            user.LastLoginDateUtc = DateTime.UtcNow;
            await _userService.UpdateUserAsync(user);

            return UserLoginResults.Successful;
        }

        /// <summary>
        /// Register user
        /// </summary>
        public async Task<UserRegistrationResult> RegisterUserAsync(UserRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.User == null)
                throw new ArgumentException("Can't load current user");

            var result = new UserRegistrationResult();

            if (request.User.IsBackgroundTaskAccount())
            {
                result.AddError("Background task account can't be registered");
                return result;
            }

            if (request.User.IsRegistered())
            {
                result.AddError("Current user is already registered");
                return result;
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                result.AddError("Email is required.");
                return result;
            }

            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError("Wrong email");
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError("Password is not provided");
                return result;
            }

            if (_userSettings.UsernamesEnabled && string.IsNullOrEmpty(request.Username))
            {
                result.AddError("Username is required.");
                return result;
            }

            //validate unique user
            if (await _userService.GetUserByEmailAsync(request.Email) != null)
            {
                result.AddError("The specified email already exists");
                return result;
            }

            if (_userSettings.UsernamesEnabled && await _userService.GetUserByUsernameAsync(request.Username) != null)
            {
                result.AddError("The specified username already exists");
                return result;
            }

            //at this point request is valid
            request.User.Username = request.Username;
            request.User.Email = request.Email;

            var userPassword = new UserPassword
            {
                User = request.User,
                PasswordFormat = request.PasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    userPassword.Password = request.Password;
                    break;
                case PasswordFormat.Encrypted:
                    userPassword.Password = _encryptionService.EncryptText(request.Password);
                    break;
                case PasswordFormat.Hashed:
                    var saltKey = _encryptionService.CreateSaltKey(UserServiceDefaults.PasswordSaltKeySize);
                    userPassword.PasswordSalt = saltKey;
                    userPassword.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _userSettings.HashedPasswordFormat);
                    break;
            }

            _userService.InsertUserPassword(userPassword);

            request.User.Active = request.IsApproved;

            //add to 'Registered' role
            var registeredRole = _userService.GetRoleBySystemName(UserDefaults.RegisteredRoleName);
            if (registeredRole == null)
                throw new DefaultException("'Registered' role could not be loaded");

            request.User.AddUserRole(new UserRole { Role = registeredRole });
            
            //remove from 'Guests' role
            var guestRole = request.User.Roles.FirstOrDefault(r => r.SystemName == UserDefaults.GuestsRoleName);
            if (guestRole != null)
            {
                request.User.RemoveUserRole(
                    request.User.UserRoles.FirstOrDefault(mapping => mapping.RoleId == guestRole.Id));    
            }

            await _userService.UpdateUserAsync(request.User);

            return result;
        }

        /// <summary>
        /// Change password
        /// </summary>
        public async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var result = new ChangePasswordResult();
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError("Email is not entered");
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError("Password is not entered");
                return result;
            }

            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                result.AddError("The specified email could not be found");
                return result;
            }

            //request isn't valid
            if (request.ValidateRequest && !PasswordsMatch(_userService.GetCurrentPassword(user.Id), request.OldPassword))
            {
                result.AddError("Old password doesn't match");
                return result;
            }

            //check for duplicates
            if (_userSettings.UnduplicatedPasswordsNumber > 0)
            {
                //get some of previous passwords
                var previousPasswords = _userService.GetUserPasswords(user.Id, passwordsToReturn: _userSettings.UnduplicatedPasswordsNumber);

                var newPasswordMatchesWithPrevious = previousPasswords.Any(password => PasswordsMatch(password, request.NewPassword));
                if (newPasswordMatchesWithPrevious)
                {
                    result.AddError("You entered the password that is the same as one of the last passwords you used. Please create a new password.");
                    return result;
                }
            }

            //at this point request is valid
            var userPassword = new UserPassword
            {
                User = user,
                PasswordFormat = request.NewPasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.NewPasswordFormat)
            {
                case PasswordFormat.Clear:
                    userPassword.Password = request.NewPassword;
                    break;
                case PasswordFormat.Encrypted:
                    userPassword.Password = _encryptionService.EncryptText(request.NewPassword);
                    break;
                case PasswordFormat.Hashed:
                    var saltKey = _encryptionService.CreateSaltKey(UserServiceDefaults.PasswordSaltKeySize);
                    userPassword.PasswordSalt = saltKey;
                    userPassword.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey,
                        request.HashedPasswordFormat ?? _userSettings.HashedPasswordFormat);
                    break;
            }

            _userService.InsertUserPassword(userPassword);

            return result;
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        public async Task SetEmailAsync(User user, string newEmail)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (newEmail == null)
                throw new DefaultException("Email cannot be null");

            newEmail = newEmail.Trim();
            var oldEmail = user.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new DefaultException("New email is not valid");

            if (newEmail.Length > 100)
                throw new DefaultException("E-mail address is too long");

            var user2 = await _userService.GetUserByEmailAsync(newEmail);
            if (user2 != null && user.Id != user2.Id)
                throw new DefaultException("The e-mail address is already in use");

            user.Email = newEmail;
            await _userService.UpdateUserAsync(user);

            if (string.IsNullOrEmpty(oldEmail) || oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase))
                return;
        }

        /// <summary>
        /// Sets a user username
        /// </summary>
        public async Task SetUsernameAsync(User user, string newUsername)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!_userSettings.UsernamesEnabled)
                throw new DefaultException("Username are disabled");

            newUsername = newUsername.Trim();

            if (newUsername.Length > UserServiceDefaults.UserUsernameLength)
                throw new DefaultException("Username is too long");

            var user2 = await _userService.GetUserByUsernameAsync(newUsername);
            if (user2 != null && user.Id != user2.Id)
                throw new DefaultException("The username is already in use");

            user.Username = newUsername;
            await _userService.UpdateUserAsync(user);
        }
    }
}