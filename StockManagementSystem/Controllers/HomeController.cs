using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Web.Controllers;
using System.Data.SqlClient;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Users;
using Microsoft.AspNetCore.Identity;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Logging;
using System.IO;

namespace StockManagementSystem.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IStoreService _storeService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserPassword> _userPasswordRepository;
        private readonly ILogger _logger;

        public HomeController(
            IStoreService storeService,
            IUserService userService,
            INotificationService notificationService,
            IConfiguration configuration,
            IGenericAttributeService genericAttributeService,
            IRepository<Role> roleRepository,
            IRepository<UserPassword> userPasswordRepository,
            ILogger logger)
        {
            _storeService = storeService;
            _userService = userService;
            _notificationService = notificationService;
            _configuration = configuration;
            _genericAttributeService = genericAttributeService;
            _roleRepository = roleRepository;
            _userPasswordRepository = userPasswordRepository;
            _logger = logger;
        }

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

        public IActionResult DownloadMasterData()
        {
            FileInfo file = null;
            SqlConnection connection = null;

            using (connection = new SqlConnection(DataSettingsManager.LoadSettings().DataConnectionString))
            {
                connection.Open();

                #region ASN Detail 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "ASNDetail.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region ASN Header 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "ASNHeader.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Barcode 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "Barcode.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Branch 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "Branch.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Item 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "Item.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Main Category 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "MainCategory.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Order Branch 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "OrderBranch.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Item 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "Role User User Role.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Sales 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "Sales.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Shift Control

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "ShiftControl.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Shelf Location 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "ShelfLocation.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Stock Take Control 

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "StockTakeControl.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Stock Take Control Outlet

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "StockTakeControlOutlet.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region SubCategory

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "SubCategory.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                #region Supplier

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "Supplier.sql"); //*.sql file path
                ExecuteScript(file, connection);

                #endregion

                connection.Close();
            }

            _notificationService.SuccessNotification("Master data successfully downloaded.");
            return View("Dashboard");
        }

        private void ExecuteScript(FileInfo file, SqlConnection connection)
        {
            string script = string.Empty;

            script = file.OpenText().ReadToEnd();
            using (SqlCommand command = new SqlCommand(script, connection))
            {
                command.ExecuteScalar();
            }
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
