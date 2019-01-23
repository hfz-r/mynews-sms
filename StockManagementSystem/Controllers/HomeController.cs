using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Web.Controllers;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System;
using StockManagementSystem.Services.Stores;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Services.Users;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using StockManagementSystem.Services.Roles;

namespace StockManagementSystem.Controllers
{
    [Authorize]
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class HomeController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRoleService _roleService;
        private readonly IStoreService _storeService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _iconfiguration;
        private readonly ILogger _logger;

        #region Constructor

        public HomeController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IRoleService roleService,
            IStoreService storeService,
            IUserService userService,
            INotificationService notificationService,
            ILoggerFactory loggerFactory,
            IConfiguration iconfiguration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleService = roleService;
            _storeService = storeService;
            _userService = userService;
            _notificationService = notificationService;
            _iconfiguration = iconfiguration;
            _logger = loggerFactory.CreateLogger<HomeController>();
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

        [AllowAnonymous]
        public IActionResult Error()
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

                var StoreList = _storeService.GetStores();
                _storeService.DeleteStore(StoreList);

                if (stores != null && stores.Count > 0)
                {
                    foreach (var item in stores)
                    {
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

                var RoleList = await _roleService.GetRolesAsync();
                foreach (var itemRoleDelete in RoleList)
                {
                    if (!itemRoleDelete.Name.ToLower().Contains("admin") &&
                        !itemRoleDelete.Name.ToLower().Contains("manager") &&
                        !itemRoleDelete.Name.ToLower().Contains("registered"))
                    {
                        await _roleService.DeleteRoleAsync(itemRoleDelete);
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
                            await _roleService.InsertRoleAsync(itemRoleAdd);
                        }
                    }
                    _logger.LogInformation(3, "Role created successfully.");
                }


                #endregion

                #region Staff

                connection.Open();

                sSQL = "SELECT [staff_no], [staff_barcode], [staff_name], [department_code], [role], [email] ";
                sSQL += "FROM [dbo].[btb_HHT_Staff]";

                using (SqlCommand command = new SqlCommand(sSQL, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        users.Add(new User()
                        {
                            UserName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(reader["staff_no"].ToString().ToLower()),
                            Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(reader["staff_name"].ToString().ToLower()),
                            AdminComment = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(reader["role"].ToString().ToLower()),
                            Email = reader["email"].ToString(),
                        });
                    }
                }
                connection.Close();

                var UserList = _userService.GetUsers();
                var filterList = UserList.Where(x => !(x.Name.Contains("Admin"))).ToList();
                if (filterList != null && filterList.Count() > 0)
                {
                    _userService.DeleteUser(filterList);
                }

                if (users != null && users.Count > 0)
                {
                    foreach (var item in users)
                    {
                        if (!string.IsNullOrEmpty(item.Email))
                        {
                            var role = item.AdminComment;
                            item.AdminComment = null;

                            var result = await _userManager.CreateAsync(item, "password123");
                            if (result.Succeeded)
                            {
                                var userRole = string.IsNullOrEmpty(role) ? "Outlet" : role == "Admin" ? "Administrators" : role;
                                await _userManager.AddToRoleAsync(item, userRole);
                            }
                            else
                            {
                                AddErrors(result);
                            }
                        }
                    }
                    _logger.LogInformation(3, "Users created a new account with default password(password123).");
                }

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
