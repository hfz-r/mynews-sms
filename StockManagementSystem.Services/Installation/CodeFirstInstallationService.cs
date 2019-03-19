using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Media;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Tenants;
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
        private readonly IRepository<Branch> _branchRepository;
        private readonly IRepository<Transaction> _transRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IWebHelper _webHelper;

        public CodeFirstInstallationService(
            IGenericAttributeService genericAttributeService, 
            IRepository<ActivityLog> activityLogRepository, 
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<Role> roleRepository,
            IRepository<User> userRepository,
            IRepository<Branch> branchRepository,
            IRepository<Transaction> transRepository,
            IRepository<Tenant> tenantRepository,
            IWebHelper webHelper)
        {
            _genericAttributeService = genericAttributeService;
            _activityLogRepository = activityLogRepository;
            _activityLogTypeRepository = activityLogTypeRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _branchRepository = branchRepository;
            _transRepository = transRepository;
            _tenantRepository = tenantRepository;
            _webHelper = webHelper;
        }

        protected void InstallTenants()
        {
            var url = _webHelper.GetLocation(false);
            var tenants = new List<Tenant>
            {
                new Tenant
                {
                    Name = "Tenant",
                    Url = url,
                    SslEnabled = false,
                    Hosts = "site.com,www.site.com",
                    DisplayOrder = 1,
                }
            };

            _tenantRepository.Insert(tenants);
        }

        protected void InstallSettings()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            settingService.SaveSetting(new CommonSettings
            {
                DisplayJavaScriptDisabledWarning = false,
                Log404Errors = true,
                StaticFilesCacheControl = "public,max-age=604800",
                UseResponseCompression = false,
                DefaultGridPageSize = 15,
                PopupGridPageSize = 10,
                GridPageSizes = "10, 15, 20, 50, 100",
                UseIsoDateFormatInJsonResult = true,
                UseNestedSetting = true,
            });

            settingService.SaveSetting(new UserSettings
            {
                UsernamesEnabled = false,
                CheckUsernameAvailabilityEnabled = false,
                AllowUsersToChangeUsernames = false,
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
                AllowUsersToUploadAvatars = false,
                AvatarMaximumSizeBytes = 20000,
                DefaultAvatarEnabled = true,
                GenderEnabled = false,
                DateOfBirthEnabled = false,
                DateOfBirthRequired = false,
                DateOfBirthMinimumAge = null,
                PhoneEnabled = false,
                PhoneRequired = false,
                StoreLastVisitedPage = false,
                StoreIpAddresses = true,
                DeleteGuestTaskOlderThanMinutes = 1440,
            });

            settingService.SaveSetting(new SecuritySettings
            {
                ForceSslForAllPages = false,
                EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
                AllowedIpAddresses = null,
                EnableXsrfProtection = true,
            });

            settingService.SaveSetting(new DateTimeSettings
            {
                DefaultTimeZoneId = string.Empty,
                AllowUsersToSetTimeZone = false,
            });

            settingService.SaveSetting(new RecordSettings
            {
                IgnoreTenantLimitations = true,
                IgnoreStoreLimitations = true,
                IgnoreAcl = true,
            });

            settingService.SaveSetting(new MediaSettings
            {
                AvatarPictureSize = 120,
                MaximumImageSize = 1980,
                DefaultImageQuality = 80,
            });
        }

        protected void InstallUsersAndRoles(string defaultUserEmail, string defaultUsername, string defaultUserPassword)
        {
            var urSysAdmin = new Role
            {
                Name = "SysAdmin",
                Active = true,
                IsSystemRole = true,
                SystemName = UserDefaults.SysAdminRoleName,
            };
            var urAdministrator = new Role
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = UserDefaults.AdministratorsRoleName,
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

            var roles = new List<Role> { urSysAdmin, urAdministrator, urRegistered, urGuests };
            _roleRepository.Insert(roles);

            //default tenant 
            var defaultTenant = _tenantRepository.Table.FirstOrDefault();

            if (defaultTenant == null)
                throw new Exception("No default tenant could be loaded");

            var tenantId = defaultTenant.Id;

            var adminUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUsername,
                Active = true,
                IsSystemAccount = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                LastLoginDateUtc = DateTime.UtcNow,
                RegisteredInTenantId = tenantId
            };

            adminUser.AddUserRole(new UserRole { Role = urSysAdmin });
            adminUser.AddUserRole(new UserRole { Role = urRegistered });

            _userRepository.Insert(adminUser);
            //set default user name
            _genericAttributeService.SaveAttributeAsync(adminUser, UserDefaults.FirstNameAttribute, "Brian").GetAwaiter().GetResult();
            _genericAttributeService.SaveAttributeAsync(adminUser, UserDefaults.LastNameAttribute, "Eno").GetAwaiter().GetResult();

            //set hashed password
            var userRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
            userRegistrationService.ChangePasswordAsync(new ChangePasswordRequest(defaultUserEmail, false,
                    PasswordFormat.Hashed, defaultUserPassword, null, UserServiceDefaults.DefaultHashedPasswordFormat))
                .GetAwaiter().GetResult();

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
                RegisteredInTenantId = tenantId
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
                    SystemKeyword = "AddNewDevice",
                    Enabled = true,
                    Name = "Add a new device"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewItem",
                    Enabled = true,
                    Name = "Add a new item"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStore",
                    Enabled = true,
                    Name = "Add a new store"
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
                    SystemKeyword = "DeleteDevice",
                    Enabled = true,
                    Name = "Delete a device"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteItem",
                    Enabled = true,
                    Name = "Delete a item"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStore",
                    Enabled = true,
                    Name = "Delete a store"
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
                    SystemKeyword = "EditDevice",
                    Enabled = true,
                    Name = "Edit a device"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditItem",
                    Enabled = true,
                    Name = "Edit a item"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStore",
                    Enabled = true,
                    Name = "Edit a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSettings",
                    Enabled = true,
                    Name = "Edit setting(s)"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMyAccount",
                    Enabled = true,
                    Name = "Edit my account"
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
                new ActivityLogType
                {
                    SystemKeyword = "DownloadStore",
                    Enabled = true,
                    Name = "Master table download - [Store]"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ClearStore",
                    Enabled = true,
                    Name = "Clear Master table data from [Store]"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DownloadRole",
                    Enabled = true,
                    Name = "Master table download - [Role]"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ClearRole",
                    Enabled = true,
                    Name = "Clear Master table data from [Role]"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DownloadUser",
                    Enabled = true,
                    Name = "Master table download - [User]"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ClearUser",
                    Enabled = true,
                    Name = "Clear Master table data from [User]"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DownloadItem",
                    Enabled = true,
                    Name = "Master table download - [Item]"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ClearItem",
                    Enabled = true,
                    Name = "Clear Master table data from [Item]"
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
            InstallTenants();
            InstallSettings();
            InstallUsersAndRoles(defaultUserEmail, defaultUsername, defaultUserPassword);
            InstallActivityLogTypes();

            if (!installSampleData)
                return;

            InstallActivityLog(defaultUserEmail);
            InstallFakerData();
        }
    }
}