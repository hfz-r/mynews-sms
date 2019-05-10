using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Geocoding.Google;
using Microsoft.Extensions.Configuration;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;
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
        private readonly IRepository<Item> _itemRepository;
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
            IRepository<Item> itemRepository,
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
            _itemRepository = itemRepository;
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
                var commandText = @"SELECT TOP 10 [outlet_no], [outlet_name], [price_level], [area_code], 
                                [address1], [address2], [address3], [postcode], [city], [state], [country] 
                                FROM [dbo].[btb_HHT_Outlet] WHERE [status] = 1";

                var reader = await SqlHelper.ExecuteReader(conn, CommandType.Text, commandText, null);
                while (reader.Read())
                {
                    var existingStore = await _storeRepository.GetByIdAsync(Convert.ToInt32(reader["outlet_no"]));
                    if (existingStore != null && (existingStore.Status == Convert.ToByte(true) || existingStore.UserStores.Any()))
                        continue;

                    var store = new Store
                    {
                        P_BranchNo = Convert.ToInt32(reader["outlet_no"]),
                        P_Name = reader["outlet_name"].ToString(),
                        P_AreaCode = reader["area_code"].ToString(),
                        P_Addr1 = reader["address1"].ToString(),
                        P_Addr2 = reader["address2"].ToString(),
                        P_Addr3 = reader["address3"].ToString(),
                        P_Postcode = Convert.ToInt32(reader["postcode"].ToString()),
                        P_City = reader["city"].ToString(),
                        P_State = reader["state"].ToString(),
                        P_Country = reader["country"].ToString()
                    };

                    if (!string.IsNullOrEmpty(store.P_Addr1) && store.P_Postcode != 0 &&
                        !string.IsNullOrEmpty(store.P_State) && !string.IsNullOrEmpty(store.P_Country))
                    {
                        var getCoordinates = await geocode.GeocodeAsync(
                            $"{store.P_Addr1}{store.P_Addr2}{store.P_Addr3}{store.P_Postcode}{store.P_City}{store.P_State}{store.P_Country}");

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
                var clearStores = 
                    from s in _storeRepository.Table
                    where s.Status != Convert.ToByte(true)
                    select s;
                if (clearStores.Any(s => s.UserStores.Count.Equals(0)))
                {
                    var deletedStores = clearStores.Count();

                    await _storeRepository.DeleteAsync(clearStores);

                    await _userActivityService.InsertActivityAsync("ClearStore", $"Deleted old master data from [Store] (Total deleted stores = {deletedStores})", new Store());
                }

                await _storeRepository.InsertAsync(stores);
                    
                await _userActivityService.InsertActivityAsync("DownloadStore", $"Downloaded master data to [Store] (Total added stores = {stores.Count})", new Store());
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
                    var existingRole = _roleRepository.Table.FirstOrDefault(x => x.Name.Equals(reader["Role"].ToString(), StringComparison.InvariantCultureIgnoreCase));
                    if (existingRole != null && existingRole.Active)
                        continue;

                    if (!reader["Role"].ToString().Contains("Admin", StringComparison.InvariantCultureIgnoreCase) &&
                        !reader["Role"].ToString().Contains("Registered", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var role = new Role
                        {
                            Name = reader["Role"].ToString(),
                            SystemName = reader["Role"].ToString(),
                        };

                        roles.Add(role);
                    }
                }
            }

            try
            {
                var clearRoles =
                    from r in _roleRepository.Table
                    where !r.IsSystemRole && !r.Active
                    select r;
                if (clearRoles.Any())
                {
                    var deletedRoles = clearRoles.Count();

                    await _roleRepository.DeleteAsync(clearRoles);

                    await _userActivityService.InsertActivityAsync("ClearRole", $"Deleted old master data from [Role] (Total deleted roles = {deletedRoles})", new Role());
                }

                await _roleRepository.InsertAsync(roles);

                await _userActivityService.InsertActivityAsync("DownloadRole", $"Downloaded master data to [Role] (Total added roles = {roles.Count})", new Role());
            }
            catch (Exception exception)
            {
                _logger.Error("An error occurred while downloading data to table [Role]", exception);
            }

            #endregion

            #region User

            try
            {
                var clearUsers = 
                    from u in _userRepository.Table
                    where !u.IsSystemAccount && !u.Active
                    select u;
                if (clearUsers.Any(u => u.UserStores.Count.Equals(0) && u.UserRoles.Count.Equals(0)))
                {
                    //remove from generic attribute
                    foreach (var user in clearUsers.ToList())
                    {
                        var genericAttributes = await _genericAttributeService.GetAttributesForEntityAsync(user.Id, typeof(User).Name);
                      
                        await _genericAttributeService.DeleteAttributes(genericAttributes.ToList());
                    }

                    var deletedUsers = clearUsers.Count();

                    await _userRepository.DeleteAsync(clearUsers);

                    await _userActivityService.InsertActivityAsync("ClearUser", $"Deleted old master data from [User] (Total deleted users = {deletedUsers})", new User());
                }

                var registeredRole = _userService.GetRoleBySystemName(UserDefaults.RegisteredRoleName);

                using (var conn = new SqlConnection(SqlHelper.ConnectionString))
                {
                    var commandText = @"SELECT TOP 10 [staff_no], [staff_barcode], [staff_name], [department_code], [role], [email] 
                                        FROM [dbo].[btb_HHT_Staff]";

                    var reader = await SqlHelper.ExecuteReader(conn, CommandType.Text, commandText, null);
                    while (reader.Read())
                    {
                        var existingUser = await _userService.GetUserByUsernameAsync(reader["staff_no"].ToString());
                        if (existingUser != null && (existingUser.Active || existingUser.UserStores.Any() || existingUser.UserRoles.Any()))
                            continue;

                        var systemName = string.IsNullOrEmpty(reader["role"].ToString()) ? "Outlet" : reader["role"].ToString() == "Admin" ? "Administrators" : reader["role"].ToString();

                        if (!string.IsNullOrWhiteSpace(reader["email"].ToString()))
                        {
                            var user = new User
                            {
                                UserGuid = Guid.NewGuid(),
                                Email = reader["email"].ToString().Trim(),
                                Username = reader["staff_no"].ToString(),
                                CreatedOnUtc = DateTime.UtcNow,
                                ModifiedOnUtc = DateTime.UtcNow,
                            };

                            var role = _userService.GetRoleBySystemName(systemName);
                            user.AddUserRole(new UserRole { Role = role });
                            user.AddUserRole(new UserRole { Role = registeredRole });

                            await _userService.InsertUserAsync(user);

                            await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.FirstNameAttribute,
                                reader["staff_name"].ToString());

                            _userPasswordRepository.Insert(new UserPassword
                            {
                                User = user,
                                Password = "12345678",
                                PasswordFormat = PasswordFormat.Clear,
                                PasswordSalt = string.Empty,
                                CreatedOnUtc = DateTime.UtcNow
                            });
                        }
                    }
                }

                await _userActivityService.InsertActivityAsync("DownloadUser", $"Downloaded master data to [User] (Total added users = {_userRepository.Table.Count()})", new User());
            }
            catch (Exception exception)
            {
                _logger.Error("An error occurred while downloading data to table [User]", exception);
            }

            #endregion

            #region Item

            var items = new List<Item>();

            using (var conn = new SqlConnection(SqlHelper.ConnectionString))
            {
                var commandText = @"SELECT TOP 10 [P_StockCode], [P_Desc], [P_SubCategoryID], [P_SPrice1], [P_SPrice2], 
                                    [P_SPrice3], [P_SPrice4], [P_SPrice5], [P_SPrice6], [P_SPrice7], [P_SPrice8],
                                    [P_SPrice9], [P_SPrice10], [P_SPrice11], [P_SPrice12], [P_SPrice13], [P_SPrice14],
                                    [P_SPrice15], [P_ModifyDT], [P_OrderStatus], [P_DisplayShelfLife] 
                                    FROM [dbo].[ItemMaster]";

                var reader = await SqlHelper.ExecuteReader(conn, CommandType.Text, commandText, null);
                while (reader.Read())
                {
                    var existingItem = _itemRepository.Table.FirstOrDefault(x => x.P_StockCode.Equals(reader["P_StockCode"].ToString(), StringComparison.InvariantCultureIgnoreCase));
                    if (existingItem != null)
                        continue;

                    var item = new Item
                    {
                        P_StockCode = reader["P_StockCode"].ToString(),
                        P_Desc = reader["P_Desc"].ToString(),
                        P_SubCategoryID = Convert.ToInt32(reader["P_SubCategoryID"]),
                        P_SPrice1 = Convert.ToDouble(reader["P_SPrice1"].ToString()),
                        P_SPrice2 = Convert.ToDouble(reader["P_SPrice2"].ToString()),
                        P_SPrice3 = Convert.ToDouble(reader["P_SPrice3"].ToString()),
                        P_SPrice4 = Convert.ToDouble(reader["P_SPrice4"].ToString()),
                        P_SPrice5 = Convert.ToDouble(reader["P_SPrice5"].ToString()),
                        P_SPrice6 = Convert.ToDouble(reader["P_SPrice6"].ToString()),
                        P_SPrice7 = Convert.ToDouble(reader["P_SPrice7"].ToString()),
                        P_SPrice8 = Convert.ToDouble(reader["P_SPrice8"].ToString()),
                        P_SPrice9 = Convert.ToDouble(reader["P_SPrice9"].ToString()),
                        P_SPrice10 = Convert.ToDouble(reader["P_SPrice10"].ToString()),
                        P_SPrice11 = Convert.ToDouble(reader["P_SPrice11"].ToString()),
                        P_SPrice12 = Convert.ToDouble(reader["P_SPrice12"].ToString()),
                        P_SPrice13 = Convert.ToDouble(reader["P_SPrice13"].ToString()),
                        P_SPrice14 = Convert.ToDouble(reader["P_SPrice14"].ToString()),
                        P_SPrice15 = Convert.ToDouble(reader["P_SPrice15"].ToString()),
                        P_ModifyDT = Convert.ToDateTime(reader["P_ModifyDT"].ToString()),
                        P_OrderStatus = Convert.ToInt32(reader["P_OrderStatus"].ToString()),
                        P_DisplayShelfLife = Convert.ToInt32(reader["P_DisplayShelfLife"].ToString())
                    };

                    items.Add(item);
                }
            }

            try
            {
                var clearItems =
                    from i in _itemRepository.Table
                    where string.IsNullOrEmpty(i.P_StockCode) && string.IsNullOrEmpty(i.P_Desc)
                    select i;
                if (clearItems.Any())
                {
                    var deletedItems = clearItems.Count();

                    await _itemRepository.DeleteAsync(clearItems);

                    await _userActivityService.InsertActivityAsync("ClearItem", $"Deleted old master data from [Item] (Total deleted items = {deletedItems})", new Item());
                }

                await _itemRepository.InsertAsync(items);

                await _userActivityService.InsertActivityAsync("DownloadItem", $"Downloaded master data to [Item] (Total added items = {items.Count})", new Item());
            }
            catch (Exception exception)
            {
                _logger.Error("An error occurred while downloading data to table [Item]", exception);
            }

            #endregion
        }

        public string Schedule => "0 17 * * *";

        public bool Enabled => false;
    }
}