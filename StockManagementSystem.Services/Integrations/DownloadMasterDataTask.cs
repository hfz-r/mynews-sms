using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Geocoding.Google;
using Microsoft.Extensions.Configuration;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Data.Extensions;
using StockManagementSystem.Data.Infrastructure;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Tasks.Scheduling;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Services.Integrations
{
    public class DownloadMasterDataTask : IScheduledTask
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserPassword> _userPasswordRepository;
        private readonly ILogger _logger;

        public DownloadMasterDataTask(
            IGenericAttributeService genericAttributeService,
            IUserService userService,
            IUserActivityService userActivityService,
            IConfiguration configuration,
            IRepository<Store> storeRepository,
            IRepository<Role> roleRepository,
            IRepository<User> userRepository,
            IRepository<UserPassword> userPasswordRepository,
            ILogger logger)
        {
            _genericAttributeService = genericAttributeService;
            _userService = userService;
            _userActivityService = userActivityService;
            _configuration = configuration;
            _storeRepository = storeRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userPasswordRepository = userPasswordRepository;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            #region Store

            var geocode = new GoogleGeocoder { ApiKey = _configuration["APIKey"] };

            var stores = new List<Store>();

            using (var conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                var commandText = @"SELECT TOP 300 [outlet_no], [outlet_name], [price_level], [area_code], 
                                [address1], [address2], [address3], [postcode], [city], [state], [country] 
                                FROM [dbo].[btb_HHT_Outlet] WHERE [status] = 1";

                var reader = await SqlHelper.ExecuteReader(conn, CommandType.Text, commandText, null);
                while (reader.Read())
                {
                    var store = new Store
                    {
                        P_BranchNo = Convert.ToInt32(reader["outlet_no"]),
                        P_Name = reader["outlet_name"].ToString(),
                        P_SellPriceLevel = reader["price_level"].ToString(),
                        P_AreaCode = reader["area_code"].ToString(),
                        P_Addr1 = reader["address1"].ToString(),
                        P_Addr2 = reader["address2"].ToString(),
                        P_Addr3 = reader["address3"].ToString(),
                        P_PostCode = reader["postcode"].ToString(),
                        P_City = reader["city"].ToString(),
                        P_State = reader["state"].ToString(),
                        P_Country = reader["country"].ToString()
                    };

                    if (!string.IsNullOrEmpty(store.P_Addr1) && !string.IsNullOrEmpty(store.P_PostCode) &&
                        !string.IsNullOrEmpty(store.P_State) && !string.IsNullOrEmpty(store.P_Country))
                    {
                        var getCoordinates = await geocode.GeocodeAsync(
                            $"{store.P_Addr1}{store.P_Addr2}{store.P_Addr3}{store.P_PostCode}{store.P_City}{store.P_State}{store.P_Country}");

                        var addresses = getCoordinates as IList<GoogleAddress> ?? getCoordinates.ToList();
                        if (addresses.Any())
                        {
                            store.Latitude = addresses.First().Coordinates.Latitude;
                            store.Longitude = addresses.First().Coordinates.Longitude;
                        }
                    }

                    stores.Add(store);
                }
            }

            try
            {
                var oldStores = _storeRepository.Table;
                if (oldStores.Any())
                {
                    await _storeRepository.DeleteAsync(oldStores);

                    await _userActivityService.InsertActivityAsync("ClearStore", "Deleted old master data from [Store]", new Store());
                }

                await _storeRepository.InsertAsync(stores);
                    
                await _userActivityService.InsertActivityAsync("DownloadStore", $"Downloaded master data to [Store] (Total stores = {stores.Count})", new Store());
            }
            catch (Exception exception)
            {
                _logger.Error("An error occurred while downloading data to table [Store]", exception);
            }

            #endregion

            #region Role

            var roles = new List<Role>();

            using (var conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                var commandText = @"SELECT DISTINCT(CASE WHEN [role] = '' OR[role] IS NULL THEN 'Outlet' ELSE[role] END) AS Role 
                                    FROM [dbo].[btb_HHT_Staff]";

                var reader = await SqlHelper.ExecuteReader(conn, CommandType.Text, commandText, null);
                while (reader.Read())
                {
                    if (!reader["Role"].ToString().Contains("Admin", StringComparison.InvariantCultureIgnoreCase) &&
                        !reader["Role"].ToString().Contains("Registered", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var role = new Role
                        {
                            Name = reader["Role"].ToString(),
                            Active = true,
                            SystemName = reader["Role"].ToString(),
                        };

                        roles.Add(role);
                    }
                }
            }

            try
            {
                var oldRoles =
                    from r in _roleRepository.Table
                    where !r.IsSystemRole
                    select r;
                if (oldRoles.Any())
                {
                    await _roleRepository.DeleteAsync(oldRoles);

                    await _userActivityService.InsertActivityAsync("ClearRole", "Deleted old master data from [Role]", new Role());
                }

                await _roleRepository.InsertAsync(roles);

                //activity log
                await _userActivityService.InsertActivityAsync("DownloadRole", $"Downloaded master data to [Role] (Total roles = {roles.Count})", new Role());
            }
            catch (Exception exception)
            {
                _logger.Error("An error occurred while downloading data to table [Role]", exception);
            }

            #endregion

            #region User

            try
            {
                var oldUsers =
                    from u in _userRepository.Table
                    where !u.IsSystemAccount
                    select u;
                if (oldUsers.Any())
                {
                    var keyGroup = oldUsers.First().GetUnproxiedEntityType().Name;
                    foreach (var user in oldUsers)
                    {
                        var genericAttributes = await _genericAttributeService.GetAttributesForEntityAsync(user.Id, keyGroup);

                        await _genericAttributeService.DeleteAttributes(genericAttributes);
                    }

                    await _userRepository.DeleteAsync(oldUsers);

                    await _userActivityService.InsertActivityAsync("ClearUser", "Deleted old master data from [User]", new User());
                }

                var registeredRole = _userService.GetRoleBySystemName(UserDefaults.RegisteredRoleName);
                var tenantId = EngineContext.Current.Resolve<ITenantContext>().CurrentTenant?.Id ?? 0;

                using (var conn = new SqlConnection(SqlHelper.ConnectionString))
                {
                    var commandText =
                        @"SELECT TOP 300 [staff_no], [staff_barcode], [staff_name], [department_code], [role], [email] 
                                FROM [dbo].[btb_HHT_Staff]";

                    var reader = await SqlHelper.ExecuteReader(conn, CommandType.Text, commandText, null);
                    while (reader.Read())
                    {
                        var systemName = string.IsNullOrEmpty(reader["role"].ToString()) ? "Outlet" : reader["role"].ToString() == "Admin"
                                ? "Administrators" : reader["role"].ToString();

                        if (!string.IsNullOrWhiteSpace(reader["email"].ToString()))
                        {
                            var user = new User
                            {
                                UserGuid = Guid.NewGuid(),
                                Email = reader["email"].ToString().Trim(),
                                Username = reader["staff_no"].ToString(),
                                Active = true,
                                CreatedOnUtc = DateTime.UtcNow,
                                RegisteredInTenantId = tenantId,
                            };

                            var role = _userService.GetRoleBySystemName(systemName);
                            user.AddUserRole(new UserRole { Role = role });
                            user.AddUserRole(new UserRole { Role = registeredRole });

                            await _userService.InsertUserAsync(user);

                            await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.FirstNameAttribute,
                                reader["staff_name"].ToString(), tenantId);

                            _userPasswordRepository.Insert(new UserPassword
                            {
                                User = user,
                                Password = "password123",
                                PasswordFormat = PasswordFormat.Clear,
                                PasswordSalt = string.Empty,
                                CreatedOnUtc = DateTime.UtcNow
                            });
                        }
                    }
                }

                //activity log
                await _userActivityService.InsertActivityAsync("DownloadUser", $"Downloaded master data to [User] (Total users = {_userRepository.Table.Count()})", new User());
            }
            catch (Exception exception)
            {
                _logger.Error("An error occurred while downloading data to table [User]", exception);
            }

            #endregion
        }

        public string Schedule => "0 0 * * *";
    }
}