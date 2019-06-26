﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Directory;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Media;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
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
        private readonly IRepository<NotificationCategory> _notificationCategoryRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Transaction> _transRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<LocalState> _stateRepository;
        private readonly IWebHelper _webHelper;

        public CodeFirstInstallationService(
            IGenericAttributeService genericAttributeService, 
            IRepository<ActivityLog> activityLogRepository, 
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<NotificationCategory> notificationCategoryRepository,
            IRepository<Role> roleRepository,
            IRepository<User> userRepository,
            IRepository<Store> storeRepository,
            IRepository<Transaction> transRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<LocalState> stateRepository,
            IWebHelper webHelper)
        {
            _genericAttributeService = genericAttributeService;
            _activityLogRepository = activityLogRepository;
            _activityLogTypeRepository = activityLogTypeRepository;
            _notificationCategoryRepository = notificationCategoryRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _transRepository = transRepository;
            _tenantRepository = tenantRepository;
            _stateRepository = stateRepository;
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
                DefaultPasswordFormat = PasswordFormat.Clear,
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
                Name = "Guest",
                Active = true,
                IsSystemRole = true,
                SystemName = UserDefaults.GuestsRoleName,
            };
            var urCashier = new Role
            {
                Name = "Cashier",
                Active = true,
                IsSystemRole = true,
                SystemName = UserDefaults.CashierRoleName,
            };

            var roles = new List<Role> { urSysAdmin, urAdministrator, urRegistered, urGuests, urCashier };
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
                    PasswordFormat.Clear, defaultUserPassword, null, UserServiceDefaults.DefaultHashedPasswordFormat))
                .GetAwaiter().GetResult();

            //built-in user for background tasks
            var backgroundTaskUser = new User
            {
                Email = "builtin@background-task-record.com",
                UserGuid = Guid.NewGuid(),
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
                    SystemKeyword = "AddNewOrderLimit",
                    Enabled = true,
                    Name = "Add a new order limit"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewPushNotification",
                    Enabled = true,
                    Name = "Add a new push notification"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTransaction",
                    Enabled = true,
                    Name = "Add a new transaction"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTransporterTransaction",
                    Enabled = true,
                    Name = "Add a new transporter transaction"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewShelfLocation",
                    Enabled = true,
                    Name = "Add a new shelf location"
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
                    SystemKeyword = "DeleteSystemLog",
                    Enabled = true,
                    Name = "Delete system log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStore",
                    Enabled = true,
                    Name = "Delete a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteOrderLimit",
                    Enabled = true,
                    Name = "Delete an order limit"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeletePushNotification",
                    Enabled = true,
                    Name = "Delete a push notification"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTransaction",
                    Enabled = true,
                    Name = "Delete a transaction"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTransporterTransaction",
                    Enabled = true,
                    Name = "Delete a transporter transaction"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteShelfLocation",
                    Enabled = true,
                    Name = "Delete a shelf location"
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
                    SystemKeyword = "EditOrderLimit",
                    Enabled = true,
                    Name = "Edit an order limit"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPushNotification",
                    Enabled = true,
                    Name = "Edit a push notification"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTransaction",
                    Enabled = true,
                    Name = "Edit a transaction"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTransporterTransaction",
                    Enabled = true,
                    Name = "Edit a transporter transaction"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditShelfLocation",
                    Enabled = true,
                    Name = "Edit a shelf location"
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

        protected void InstallNotificationCategory()
        {
            var notificationCategory = new List<NotificationCategory>
            {
                new NotificationCategory
                {
                    Code = "ST",
                    Name = "Stock Take"
                },
                new NotificationCategory
                {
                    Code = "OT",
                    Name = "Others"
                },
            };
            _notificationCategoryRepository.Insert(notificationCategory);
        }

        //protected void InstallStatesAndHolidays()
        //{
        //    var states = new List<LocalState>
        //    {
        //        new LocalState
        //        {
        //            Description = "",
        //            Abbreviation = "",
        //            IsSameWeekend = null,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Chinese Lunar New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 2, 5),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Second day of Chinese Lunar New Year",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 2, 6),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Labour Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 1),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Wesak Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 19),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Wesak Day observed",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 20),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Hari Raya Puasa",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 6, 5),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Hari Raya Puasa Day 2",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 6, 6),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Hari Raya Haji",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 8, 11),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Malaysia's National Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 8, 31),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Muharram/New Year",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 9, 1),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "The Yang di-Pertuan Agong's Birthday",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 9, 9),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Malaysia Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 9, 16),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "The Prophet Muhammad's Birthday",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 11, 9),
        //                },
        //                new Holiday
        //                {
        //                    Type = "Federal",
        //                    Description = "Christmas Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 12, 25),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "JOHOR",
        //            Abbreviation = "JHR",
        //            IsSameWeekend = false,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Sultan of Johor",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 3, 23),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Almarhum Sultan Iskandar Hol Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 5),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Ramadan begins",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 6),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Thaipusam",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 21),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "KEDAH",
        //            Abbreviation = "KDH",
        //            IsSameWeekend = false,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Isra and Mi'raj",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 4, 3),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Ramadan begins",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 6),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Hari Raya Haji (Day 2)",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 8, 12),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "KELANTAN",
        //            Abbreviation = "KTN",
        //            IsSameWeekend = false,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Sultan of Kelantan",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 11, 11),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Sultan of Kelantan (Day 2)",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 11, 12),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Hari Raya Haji (Day 2)",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 8, 12),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Nuzul Al-Quran",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 22),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "MELAKA",
        //            Abbreviation = "MLK",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Declaration of Malacca as Historical City",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 4, 15),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Governor of Malacca",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 11),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Ramadan begins",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 6),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "NEGERI SEMBILAN",
        //            Abbreviation = "NSN",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of Yang di-Pertuan Besar",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 14),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Isra and Mi'raj",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 4, 3),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Thaipusam",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 21),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "PAHANG",
        //            Abbreviation = "PHG",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Pahang State Holiday",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 7),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Sultan of Pahang",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 24),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Nuzul Al-Quran",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 22),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "PULAU PINANG",
        //            Abbreviation = "PNG",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "George Town World Heritage City Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 7, 7),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Penang Governor's Birthday",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 7, 13),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Thaipusam",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 21),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Nuzul Al-Quran",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 22),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "PERAK",
        //            Abbreviation = "PRK",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Sultan of Perak",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 11, 1),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Thaipusam",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 21),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Nuzul Al-Quran",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 22),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "PERLIS",
        //            Abbreviation = "PLS",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Raja of Perlis",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 7, 17),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Isra and Mi'raj",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 4, 3),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Hari Raya Haji (Day 2)",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 8, 12),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Nuzul Al-Quran",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 12),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "SELANGOR",
        //            Abbreviation = "SGR",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Sultan of Selangor",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 12, 11),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Thaipusam",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 21),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Nuzul Al-Quran",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 22),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "TERENGGANU",
        //            Abbreviation = "TRG",
        //            IsSameWeekend = false,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Anniversary of the coronation of the Sultan of Terengganu",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 3, 4),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Sultan of Terengganu",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 4, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Hari Raya Haji (Day 2)",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 8, 12),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Nuzul Al-Quran",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 22),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "SABAH",
        //            Abbreviation = "SBH",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Governor of Sabah",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 5),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Good Friday",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 4, 19),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Harvest Festival",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 30),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Harvest Festival Day 2",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 31),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "SARAWAK",
        //            Abbreviation = "SWK",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Gawai Dayak",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 6, 1),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Gawai Dayak Holiday",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 6, 2),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Gawai Dayak Holiday observed",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 6, 3),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Sarawak Independence Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 7, 22),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Birthday of the Governor of Sarawak",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 12),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Good Friday",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 4, 19),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "KUALA LUMPUR",
        //            Abbreviation = "KUL",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Federal Territory Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 2, 11),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Thaipusam",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 21),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Nuzul Al-Quran",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 22),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "LABUAN",
        //            Abbreviation = "LBN",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Harvest Festival",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 30),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Harvest Festival Day 2",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 31),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Federal Territory Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 2, 1),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Nuzul Al-Quran",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 22),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //        new LocalState
        //        {
        //            Description = "PUTRAJAYA",
        //            Abbreviation = "PJY",
        //            IsSameWeekend = true,
        //            Holidays = new List<Holiday>
        //            {
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Federal Territory Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 2, 1),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Thaipusam",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 21),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Nuzul Al-Quran",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 5, 22),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 27),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "Diwali/Deepavali",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 10, 28),
        //                },
        //                new Holiday
        //                {
        //                    Type = "State",
        //                    Description = "New Year's Day",
        //                    FullDateTime = new DateTime(DateTime.Now.Year, 1, 1),
        //                },
        //            }
        //        },
        //    };

        //    _stateRepository.Insert(states);
        //}

        #region Faker data

        protected void InstallFakerData()
        {
            SeedStore();
            SeedTransaction();
        }

        protected void SeedStore()
        {
            var storesFaker = new Faker<Store>()
                .RuleFor(s => s.P_BranchNo, s=> s.Random.Number(1000, 10000))
                .RuleFor(s=> s.P_Name, s => s.Company.CompanyName())
                .RuleFor(s => s.P_AreaCode, s => s.Address.CountryCode())
                .RuleFor(s => s.P_Addr1, s => s.Address.FullAddress())
                .RuleFor(s => s.P_State, s => s.Address.State())
                .RuleFor(s => s.P_City, s => s.Address.City())
                .RuleFor(s => s.P_Country, s => s.Address.Country())
                .RuleFor(s => s.Latitude, s => s.Address.Latitude())
                .RuleFor(s => s.Longitude, s => s.Address.Longitude());

            var stores = storesFaker.Generate(50);

            _storeRepository.Insert(stores);
        }

        protected void SeedTransaction()
        {
            var categories = new[] { "Stock Transfer Out", "Stock Transfer In", "Stock Order", "Stock Receive", "Stock Adjustment" };

            var transFaker = new Faker<Transaction>()
                .RuleFor(t => t.P_StockCode, f => f.PickRandom(categories))
                .RuleFor(t => t.CreatedOnUtc, f => f.Date.Between(new DateTime(2000, 1, 1), new DateTime(2018, 12, 1)))
                .RuleFor(t => t.Store, f => f.PickRandom(_storeRepository.Table?.ToList()))
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
            InstallNotificationCategory();
            InstallActivityLogTypes();
            //InstallStatesAndHolidays(); //Pass by EDI

            if (!installSampleData)
                return;

            InstallActivityLog(defaultUserEmail);
            InstallFakerData();
        }
    }
}