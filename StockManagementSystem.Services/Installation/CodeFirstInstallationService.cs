﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Tasks;
using StockManagementSystem.Core.Domain.Transactions;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Services.Installation
{
    public class CodeFirstInstallationService : IInstallationService
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserPassword> _userPasswordRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Branch> _branchRepository;
        private readonly IRepository<Transaction> _transRepository;
        private readonly IWebHelper _webHelper;

        public CodeFirstInstallationService(
            IGenericAttributeService genericAttributeService, 
            IRepository<ActivityLog> activityLogRepository, 
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<Role> roleRepository,
            IRepository<User> userRepository,
            IRepository<UserPassword> userPasswordRepository,
            IRepository<ScheduleTask> scheduleTaskRepository,
            IRepository<Store> storeRepository,
            IRepository<Branch> branchRepository,
            IRepository<Transaction> transRepository,
            IWebHelper webHelper)
        {
            _genericAttributeService = genericAttributeService;
            _activityLogRepository = activityLogRepository;
            _activityLogTypeRepository = activityLogTypeRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userPasswordRepository = userPasswordRepository;
            _scheduleTaskRepository = scheduleTaskRepository;
            _storeRepository = storeRepository;
            _branchRepository = branchRepository;
            _transRepository = transRepository;
            _webHelper = webHelper;
        }

        protected void InstallStores()
        {
            var storeUrl = _webHelper.GetStoreLocation(false);
            var stores = new List<Store>
            {
                new Store
                {
                    P_BranchNo = 1,
                    P_Name = "Default Store",
                    Url = storeUrl,
                    SslEnabled = false,
                    Hosts = "defaultstore.com,www.defaultstore.com",
                }
            };

            _storeRepository.Insert(stores);
        }

        protected void InstallSettings()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            settingService.SaveSetting(new UserSettings
            {
                UsernamesEnabled = true,
                CheckUsernameAvailabilityEnabled = true,
                DefaultPasswordFormat = PasswordFormat.Hashed,
                HashedPasswordFormat = UserServiceDefaults.DefaultHashedPasswordFormat,
                PasswordMinLength = 6,
                PasswordRequireDigit = false,
                PasswordRequireLowercase = false,
                PasswordRequireNonAlphanumeric = false,
                PasswordRequireUppercase = false,
                UnduplicatedPasswordsNumber = 4,
                PasswordRecoveryLinkDaysValid = 7,
                PasswordLifetime = 90,
                FailedPasswordAllowedAttempts = 0,
                FailedPasswordLockoutMinutes = 30,
                GenderEnabled = false,
                DateOfBirthEnabled = false,
                DateOfBirthRequired = false,
                PhoneEnabled = false,
                PhoneRequired = false,
                StoreLastVisitedPage = false,
                StoreIpAddresses = true,
                DeleteGuestTaskOlderThanMinutes = 1440,
            });

            settingService.SaveSetting(new SecuritySettings
            {
                ForceSslForAllPages = true,
                EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
                EnableXsrfProtection = true,
            });

            settingService.SaveSetting(new DateTimeSettings
            {
                DefaultStoreTimeZoneId = string.Empty,
                AllowUsersToSetTimeZone = false
            });
        }

        protected void InstallUsersAndRoles(string defaultUserEmail, string defaultUsername, string defaultUserPassword)
        {
            var urAdministrator = new Role
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = UserDefaults.AdministratorsRoleName,
            };
            var urManager = new Role
            {
                Name = "Managers",
                Active = true,
                IsSystemRole = true,
                SystemName = UserDefaults.ManagersRoleName,
            };
            var urRegistered = new Role
            {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = UserDefaults.RegisteredRoleName,
            };
            var urGuests = new Role
            {
                Name = "Guests",
                Active = true,
                IsSystemRole = true,
                SystemName = UserDefaults.GuestsRoleName,
            };

            var roles = new List<Role> { urAdministrator, urManager, urRegistered, urGuests };
            _roleRepository.Insert(roles);

            //default store 
            var defaultStore = _storeRepository.Table.FirstOrDefault();

            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            var storeBranchNo = defaultStore.P_BranchNo;

            //admin user
            var adminUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUsername,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                LastLoginDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeBranchNo,
            };

            adminUser.AddUserRole(new UserRole { Role = urAdministrator });
            adminUser.AddUserRole(new UserRole { Role = urRegistered });

            _userRepository.Insert(adminUser);
            //set default user name
            _genericAttributeService.SaveAttributeAsync(adminUser, UserDefaults.FirstNameAttribute, "John").GetAwaiter().GetResult();
            _genericAttributeService.SaveAttributeAsync(adminUser, UserDefaults.LastNameAttribute, "Katak").GetAwaiter().GetResult();

            //set hashed admin password
            var userRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
            userRegistrationService.ChangePasswordAsync(new ChangePasswordRequest(defaultUserEmail, false,
                    PasswordFormat.Hashed, defaultUserPassword, null, UserServiceDefaults.DefaultHashedPasswordFormat))
                .GetAwaiter().GetResult();

            //manager user
            var managerDefaultEmail = "manager@mynewstore.com";
            var managerDefaultUserName = "mgr";
            var managerUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = managerDefaultEmail,
                Username = managerDefaultUserName,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                LastLoginDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeBranchNo,
            };

            managerUser.AddUserRole(new UserRole { Role = urManager });
            managerUser.AddUserRole(new UserRole { Role = urRegistered });

            _userRepository.Insert(managerUser);

            //set manager name
            _genericAttributeService.SaveAttributeAsync(managerUser, UserDefaults.FirstNameAttribute, "Nicky").GetAwaiter().GetResult();
            _genericAttributeService.SaveAttributeAsync(managerUser, UserDefaults.LastNameAttribute, "Santos").GetAwaiter().GetResult();

            //set manager password
            _userPasswordRepository.Insert(new UserPassword
            {
                User = managerUser,
                Password = "mgr123",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //second user
            var secondUserEmail = "steve_gates@user.com";
            var secondUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = secondUserEmail,
                Username = secondUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeBranchNo,
            };

            secondUser.AddUserRole(new UserRole { Role = urRegistered });

            _userRepository.Insert(secondUser);
            //set default user name
            _genericAttributeService.SaveAttributeAsync(secondUser, UserDefaults.FirstNameAttribute, "Steve").GetAwaiter().GetResult();
            _genericAttributeService.SaveAttributeAsync(secondUser, UserDefaults.LastNameAttribute, "Gates").GetAwaiter().GetResult();

            //set user password
            _userPasswordRepository.Insert(new UserPassword
            {
                User = secondUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //third user
            var thirdUserEmail = "arthur_holmes@nothing.com";
            var thirdUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = thirdUserEmail,
                Username = thirdUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeBranchNo,
            };

            thirdUser.AddUserRole(new UserRole { Role = urRegistered });

            _userRepository.Insert(thirdUser);
            //set default user name
            _genericAttributeService.SaveAttributeAsync(thirdUser, UserDefaults.FirstNameAttribute, "Arthur").GetAwaiter().GetResult();
            _genericAttributeService.SaveAttributeAsync(thirdUser, UserDefaults.LastNameAttribute, "Holmes").GetAwaiter().GetResult();

            //set user password
            _userPasswordRepository.Insert(new UserPassword
            {
                User = thirdUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //built-in user for background tasks
            var backgroundTaskUser = new User
            {
                Email = "builtin@background-task-record.com",
                UserGuid = Guid.NewGuid(),
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = UserDefaults.BackgroundTaskUserName,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeBranchNo,
            };

            backgroundTaskUser.AddUserRole(new UserRole {Role = urGuests});
            _userRepository.Insert(backgroundTaskUser);
        }

        protected void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>
            {
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUser",
                    Enabled = true,
                    Name = "Add a new user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewRole",
                    Enabled = true,
                    Name = "Add a new role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUser",
                    Enabled = true,
                    Name = "Delete a user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteRole",
                    Enabled = true,
                    Name = "Delete a role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUser",
                    Enabled = true,
                    Name = "Edit a user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditRole",
                    Enabled = true,
                    Name = "Edit a role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Login",
                    Enabled = true,
                    Name = "Login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "FirstTimeLogin",
                    Enabled = true,
                    Name = "First time login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Logout",
                    Enabled = true,
                    Name = "Logout"
                },
                new ActivityLogType
                {
                    SystemKeyword = "InstallNewPlugin",
                    Enabled = true,
                    Name = "Install a new plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditApiSettings",
                    Enabled = true,
                    Name = "Edit api settings"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewApiClient",
                    Enabled = true,
                    Name = "Add a new api client"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditApiClient",
                    Enabled = true,
                    Name = "Edit api client"
                },
            };
            _activityLogTypeRepository.Insert(activityLogTypes);
        }

        protected void InstallActivityLog(string email)
        {
            var defaultUser = _userRepository.Table.FirstOrDefault(x => x.Email == email);
            if (defaultUser == null)
                throw new Exception("Cannot load default user");

            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("DeleteRole")),
                Comment = "Deleted a role",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("DeleteActivityLog")),
                Comment = "Deleted a activity log",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("AddNewRole")),
                Comment = "Added a new role ('Manager')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
        }

        protected void InstallScheduleTasks()
        {
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "StockManagementSystem.Services.Common.KeepAliveTask, StockManagementSystem.Services",
                    Enabled = true,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "StockManagementSystem.Services.Users.DeleteGuestsTask, StockManagementSystem.Services",
                    Enabled = true,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "StockManagementSystem.Services.Caching.ClearCacheTask, StockManagementSystem.Services",
                    Enabled = false,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "StockManagementSystem.Services.Logging.ClearLogTask, StockManagementSystem.Services",
                    Enabled = false,
                    StopOnError = false
                },
            };

            _scheduleTaskRepository.Insert(tasks);
        }

        #region Faker data

        protected void InstallFakerData()
        {
            SeedBranch();
            SeedTransaction();
        }

        protected void SeedBranch()
        {
            var branches = new List<Branch>
            {
                new Branch
                {
                    Name = "One Utama",
                    Location = "Selangor"
                },
                new Branch
                {
                    Name = "Ipoh Parade",
                    Location = "Perak"
                },
                new Branch
                {
                    Name = "Alor Star Mall",
                    Location = "Kedah"
                },
                new Branch
                {
                    Name = "IKEA Cheras",
                    Location = "Kuala Lumpur"
                },
                new Branch
                {
                    Name = "Kuantan Mall",
                    Location = "Pahang"
                },
            };
            _branchRepository.Insert(branches);
        }

        protected void SeedTransaction()
        {
            var categories = new[] { "Stock Transfer Out", "Stock Transfer In", "Stock Order", "Stock Receive", "Stock Adjustment" };

            var transFaker = new Faker<Transaction>()
                .RuleFor(t => t.Category, f => f.PickRandom(categories))
                .RuleFor(t => t.CreatedOnUtc, f => f.Date.Between(new DateTime(2000, 1, 1), new DateTime(2018, 12, 1)))
                .RuleFor(t => t.Branch, f => f.PickRandom(_branchRepository.Table.ToList()))
                .FinishWith((f, t) => Console.WriteLine($"Transaction created. Id={t.Id}"));

            var trans = transFaker.Generate(300);

            _transRepository.Insert(trans);
        }

        #endregion

        public void InstallData(string defaultUserEmail, string defaultUsername, string defaultUserPassword, bool installSampleData = true)
        {
            InstallStores();
            InstallSettings();
            InstallUsersAndRoles(defaultUserEmail, defaultUsername, defaultUserPassword);
            InstallActivityLogTypes();
            InstallScheduleTasks();

            if (!installSampleData)
                return;

            InstallActivityLog(defaultUserEmail);
            InstallFakerData();
        }
    }
}