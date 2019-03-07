using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Web.Controllers;
using System.Data.SqlClient;
using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System;
using StockManagementSystem.Services.Stores;
using System.Threading.Tasks;
using StockManagementSystem.Services.Users;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Geocoding;
using Geocoding.Google;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Logging;

namespace StockManagementSystem.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IStoreService _storeService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _iconfiguration;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserPassword> _userPasswordRepository;
        private readonly ILogger _logger;

        #region Constructor

        public HomeController(
            IStoreService storeService,
            IUserService userService,
            INotificationService notificationService,
            IConfiguration iconfiguration,
            IGenericAttributeService genericAttributeService,
            IRepository<Role> roleRepository,
            IRepository<UserPassword> userPasswordRepository,
            ILogger logger)
        {
            _storeService = storeService;
            _userService = userService;
            _notificationService = notificationService;
            _iconfiguration = iconfiguration;
            _genericAttributeService = genericAttributeService;
            _roleRepository = roleRepository;
            _userPasswordRepository = userPasswordRepository;
            _logger = logger;
        }

        #endregion
        public IActionResult Index()
        {
            return View("Dashboard");
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult PanelServerComponent()
        {
            return ViewComponent("PanelServerComponent");
        }

        public IActionResult PanelTxnApprovalComponent()
        {
            return ViewComponent("PanelTxnApprovalComponent");
        }

        public async Task<IActionResult> DownloadMasterData()
        {
            string conString = ConfigurationExtensions.GetConnectionString(this._iconfiguration, "HQ");
            List<Store> stores = new List<Store>();
            List<User> users = new List<User>();
            List<Role> roles = new List<Role>();
            string sSQL = string.Empty;

            using (SqlConnection connection = new SqlConnection(conString))
            {
                connection.Open();

                #region Store

                sSQL = "SELECT [outlet_no], [outlet_name], [price_level], [area_code], ";
                sSQL += "[address1], [address2], [address3], [postcode], [city], [state], [country] ";
                sSQL += "FROM [dbo].[btb_HHT_Outlet] WHERE [status] = 1";

                using (SqlCommand command = new SqlCommand(sSQL, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        stores.Add(new Store()
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
                        });
                    }
                }
                var storeList = await _storeService.GetStores();
                await _storeService.DeleteStore(storeList);

                if (stores != null && stores.Count > 0)
                {
                    IGeocoder geocoder = new GoogleGeocoder() { ApiKey = _iconfiguration["APIKey"].ToString() };

                    foreach (var item in stores)
                    {
                        if (!string.IsNullOrEmpty(item.P_Addr1) && !string.IsNullOrEmpty(item.P_PostCode) && !string.IsNullOrEmpty(item.P_State) && !string.IsNullOrEmpty(item.P_Country))
                        {
                            string address = item.P_Addr1 + item.P_Addr2 + item.P_Addr3 + item.P_PostCode + item.P_City + item.P_State + item.P_Country;
                            IEnumerable<Address> addresses = await geocoder.GeocodeAsync(address);

                            if (addresses != null && addresses.Count() > 0)
                            {
                                item.Latitude = addresses.First().Coordinates.Latitude;
                                item.Longitude = addresses.First().Coordinates.Longitude;
                            }
                        }

                        await _storeService.InsertStore(item);
                    }
                }
                connection.Close();

                #endregion

                #region Role

                connection.Open();

                sSQL = "SELECT DISTINCT(CASE WHEN [role] = '' OR[role] IS NULL THEN 'Outlet' ELSE[role] END) AS Role ";
                sSQL += "FROM [dbo].[btb_HHT_Staff]";

                using (SqlCommand command = new SqlCommand(sSQL, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        roles.Add(new Role()
                        {
                            Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(reader["Role"].ToString().ToLower()),
                            SystemName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(reader["Role"].ToString().ToLower())
                        });
                    }
                }
                connection.Close();

                //Don't use cache-able service for data seed.
                //var RoleList = _userService.GetRoles();
                var RoleList = _roleRepository.Table;
                foreach (var itemRoleDelete in RoleList)
                {
                    if (!itemRoleDelete.Name.ToLower().Contains("admin") &&
                        !itemRoleDelete.Name.ToLower().Contains("manager") &&
                        !itemRoleDelete.Name.ToLower().Contains("registered"))
                    {
                        await _userService.DeleteRoleAsync(itemRoleDelete);
                    }
                }

                if (roles != null && roles.Count > 0)
                {
                    foreach (var itemRoleAdd in roles)
                    {
                        if (!itemRoleAdd.Name.Contains("Admin") &&
                            !itemRoleAdd.Name.Contains("Manager") &&
                            !itemRoleAdd.Name.Contains("Registered"))
                        {
                            await _userService.InsertRoleAsync(itemRoleAdd);
                        }
                    }
                    _logger.Information("Role created successfully.");
                }


                #endregion

                #region Staff

                connection.Open();

                sSQL = "SELECT [staff_no], [staff_barcode], [staff_name], [department_code], [role], [email] ";
                sSQL += "FROM [dbo].[btb_HHT_Staff]";

                var UserList = _userService.GetUsers();
                var filterList = UserList.Where(x => !(x.Username.Contains("Admin"))).ToList();
                if (filterList != null && filterList.Count() > 0)
                {
                    _userService.DeleteUser(filterList);
                }

                using (SqlCommand command = new SqlCommand(sSQL, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if (!string.IsNullOrEmpty(reader["email"].ToString()))
                        {
                            var user = new User
                            {
                                UserGuid = Guid.NewGuid(),
                                Username = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(reader["staff_no"].ToString().ToLower()),
                                Email = reader["email"].ToString().Trim(),
                                //AdminComment = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(reader["role"].ToString().ToLower()),
                                //Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(reader["staff_name"].ToString().ToLower()),
                                Active = true,
                                CreatedOnUtc = DateTime.UtcNow,
                            };

                            var getRoleStr = string.IsNullOrEmpty(reader["role"].ToString())
                                ? "Outlet" : reader["role"].ToString() == "Admin"
                                    ? "Administrators" : reader["role"].ToString();
                            var role = _userService.GetRoleBySystemName(getRoleStr);

                            user.AddUserRole(new UserRole { Role = role });

                            try
                            {
                                await _userService.InsertUserAsync(user);
                                await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.FirstNameAttribute,
                                    reader["staff_name"].ToString());

                                _userPasswordRepository.Insert(new UserPassword
                                {
                                    User = user,
                                    Password = "password123",
                                    PasswordFormat = PasswordFormat.Clear,
                                    PasswordSalt = string.Empty,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                }

                _logger.Information("Users created a new account with default password(password123).");

                connection.Close();

                #endregion
            }

            _notificationService.SuccessNotification("Master data successfully downloaded.");
            return View("Dashboard");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
