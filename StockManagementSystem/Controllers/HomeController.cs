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
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Common;
using StockManagementSystem.Services.Security;

namespace StockManagementSystem.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IStoreService _storeService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserPassword> _userPasswordRepository;
        private readonly ILogger _logger;

        public HomeController(
            IStoreService storeService,
            IUserService userService,
            INotificationService notificationService,
            IConfiguration configuration,
            IGenericAttributeService genericAttributeService,
            ICommonModelFactory commonModelFactory,
            IPermissionService permissionService,
            IRepository<Role> roleRepository,
            IRepository<UserPassword> userPasswordRepository,
            ILogger logger)
        {
            _storeService = storeService;
            _userService = userService;
            _notificationService = notificationService;
            _configuration = configuration;
            _genericAttributeService = genericAttributeService;
            _commonModelFactory = commonModelFactory;
            _permissionService = permissionService;
            _roleRepository = roleRepository;
            _userPasswordRepository = userPasswordRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //display a warning to admin if there are some error
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            {
                var warnings = await _commonModelFactory.PrepareSystemWarningModels();
                if (warnings.Any(warning => warning.Level == SystemWarningLevel.Fail || warning.Level == SystemWarningLevel.Warning))
                    _notificationService.WarningNotification(
                        "The web has some error(s) or warning(s). Please find more information on the Warnings page.");
            }

            return View("Dashboard");
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

                file = new FileInfo(_configuration["ScriptFolder"].ToString() + "RoleUserUserRole.sql"); //*.sql file path
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
