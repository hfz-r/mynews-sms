using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Events;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Users;
using Tests;

namespace Services.Tests.Users
{
    public class UserRegistrationServiceTests : ServiceTest
    {
        private UserSettings _userSettings;
        private SecuritySettings _securitySettings;
        private Mock<IRepository<User>> _userRepository;
        private Mock<IRepository<UserPassword>> _userPasswordRepository;
        private Mock<IRepository<Role>> _roleRepository;
        private Mock<IRepository<UserRole>> _userRoleRepository;
        private Mock<IRepository<GenericAttribute>> _genericAttributeRepository;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Mock<IEventPublisher> _eventPublisherService;
        private Mock<IWorkContext> _workContext;
        private EncryptionService _encryptionService;
        private UserService _userService;
        private UserRegistrationService _userRegistrationService;

        [SetUp]
        public new void SetUp()
        {
            _userSettings = new UserSettings
            {
                UnduplicatedPasswordsNumber = 1,
                HashedPasswordFormat = "SHA512",
            };

            _securitySettings = new SecuritySettings
            {
                EncryptionKey = "273ece6f97dd844d"
            };

            _encryptionService = new EncryptionService(_securitySettings);

            #region User Setup 

            _userRepository = new Mock<IRepository<User>>();

            var user1 = new User
            {
                Id = 1,
                Username = "a@b.com",
                Email = "a@b.com",
                Active = true
            };
            AddUserToRegisteredRole(user1);

            var user2 = new User
            {
                Id = 2,
                Username = "test@test.com",
                Email = "test@test.com",
                Active = true
            };
            AddUserToRegisteredRole(user2);

            var user3 = new User
            {
                Id = 3,
                Username = "user@test.com",
                Email = "user@test.com",
                Active = true
            };
            AddUserToRegisteredRole(user3);

            var user4 = new User
            {
                Id = 4,
                Username = "registered@test.com",
                Email = "registered@test.com",
                Active = true
            };
            AddUserToRegisteredRole(user4);

            var user5 = new User
            {
                Id = 5,
                Username = "notregistered@test.com",
                Email = "notregistered@test.com",
                Active = true
            };

            var mockUsers = new List<User> {user1, user2, user3, user4, user5}.AsQueryable().BuildMockDbSet();
            _userRepository.Setup(x => x.Table).Returns(mockUsers.Object);

            #endregion

            #region UserPassword Setup

            _userPasswordRepository = new Mock<IRepository<UserPassword>>();

            var saltKey = _encryptionService.CreateSaltKey(5);

            var password = _encryptionService.CreatePasswordHash("password", saltKey, "SHA512");

            var password1 = new UserPassword
            {
                UserId = user1.Id,
                PasswordFormat = PasswordFormat.Hashed,
                PasswordSalt = saltKey,
                Password = password,
                CreatedOnUtc = DateTime.UtcNow
            };

            var password2 = new UserPassword
            {
                UserId = user2.Id,
                PasswordFormat = PasswordFormat.Clear,
                Password = "password",
                CreatedOnUtc = DateTime.UtcNow
            };

            var password3 = new UserPassword
            {
                UserId = user3.Id,
                PasswordFormat = PasswordFormat.Encrypted,
                Password = _encryptionService.EncryptText("password"),
                CreatedOnUtc = DateTime.UtcNow
            };

            var password4 = new UserPassword
            {
                UserId = user4.Id,
                PasswordFormat = PasswordFormat.Clear,
                Password = "password",
                CreatedOnUtc = DateTime.UtcNow
            };

            var password5 = new UserPassword
            {
                UserId = user5.Id,
                PasswordFormat = PasswordFormat.Clear,
                Password = "password",
                CreatedOnUtc = DateTime.UtcNow
            };

            var mockUserPasswords = new[] {password1, password2, password3, password4, password5}
                .AsQueryable().BuildMockDbSet();
            _userPasswordRepository.Setup(x => x.Table).Returns(mockUserPasswords.Object);

            #endregion

            _roleRepository = new Mock<IRepository<Role>>();
            _userRoleRepository = new Mock<IRepository<UserRole>>();
            _genericAttributeRepository = new Mock<IRepository<GenericAttribute>>();
            _genericAttributeService = new Mock<IGenericAttributeService>();
            _workContext = new Mock<IWorkContext>();
            _eventPublisherService = new Mock<IEventPublisher>();

            _userService = new UserService(
                new UserSettings(), 
                new NullCache(),
                null,
                null,
                _eventPublisherService.Object,
                _genericAttributeService.Object, 
                _userRepository.Object, 
                _roleRepository.Object, 
                _userRoleRepository.Object, 
                _userPasswordRepository.Object, 
                _genericAttributeRepository.Object, 
                null);

            _userRegistrationService = new UserRegistrationService(_userSettings, _userService, _encryptionService);
        }

        [Test]
        public void Only_registered_users_can_login()
        {
            var result = _userRegistrationService.ValidateUserAsync("registered@test.com", "password").Result;
            result.ShouldEqual(UserLoginResults.Successful);

            result = _userRegistrationService.ValidateUserAsync("notregistered@test.com", "password").Result;
            result.ShouldEqual(UserLoginResults.NotRegistered);
        }

        [Test]
        public void Can_validate_a_hashed_password()
        {
            var result = _userRegistrationService.ValidateUserAsync("a@b.com", "password").Result;
            result.ShouldEqual(UserLoginResults.Successful);
        }

        [Test]
        public void Can_validate_a_clear_password()
        {
            var result = _userRegistrationService.ValidateUserAsync("test@test.com", "password").Result;
            result.ShouldEqual(UserLoginResults.Successful);
        }

        [Test]
        public void Can_validate_an_encrypted_password()
        {
            var result = _userRegistrationService.ValidateUserAsync("user@test.com", "password").Result;
            result.ShouldEqual(UserLoginResults.Successful);
        }

        [Test]
        public void Can_change_password()
        {
            var request = new ChangePasswordRequest("registered@test.com", true, PasswordFormat.Clear, "password", "password");
            var result = _userRegistrationService.ChangePasswordAsync(request).Result;
            result.Success.ShouldEqual(false);

            request = new ChangePasswordRequest("registered@test.com", true, PasswordFormat.Hashed, "newpassword", "password");
            result = _userRegistrationService.ChangePasswordAsync(request).Result;
            result.Success.ShouldEqual(true);
        }

        #region Private methods

        private void AddUserToRegisteredRole(User user)
        {
            user.Roles.Add(new Role
            {
                Active = true,
                IsSystemRole = true,
                SystemName = UserDefaults.RegisteredRoleName
            });
        }

        #endregion
    }
}