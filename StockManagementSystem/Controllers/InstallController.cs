using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Plugins;
using StockManagementSystem.Models.Install;
using StockManagementSystem.Services.Installation;
using StockManagementSystem.Services.Plugins;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Security;

namespace StockManagementSystem.Controllers
{
    public class InstallController : Controller
    {
        private readonly IFileProviderHelper _fileProvider;
        private readonly DefaultConfig _config;

        public InstallController(IFileProviderHelper fileProvider, DefaultConfig config)
        {
            _fileProvider = fileProvider;
            _config = config;
        }

        #region Utilities

        /// <summary>
        /// A value indicating whether we use MARS (Multiple Active Result Sets)
        /// </summary>
        protected bool UseMars => false;

        protected bool SqlServerDatabaseExists(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected string CreateDatabase(string connectionString, string collation, int triesToConnect = 10)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);

                var databaseName = builder.InitialCatalog;
                builder.InitialCatalog = "master";
                var masterCatalogConnectionString = builder.ToString();

                var query = $"CREATE DATABASE [{databaseName}]";

                if (!string.IsNullOrWhiteSpace(collation))
                    query = $"{query} COLLATE {collation}";

                using (var conn = new SqlConnection(masterCatalogConnectionString))
                {
                    conn.Open();
                    using (var command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                //try connect
                if (triesToConnect > 0)
                {
                    for (var i = 0; i <= triesToConnect; i++)
                    {
                        if (i == triesToConnect)
                            throw new Exception("Unable to connect to the new database. Please try one more time");

                        if (!this.SqlServerDatabaseExists(connectionString))
                            Thread.Sleep(1000);
                        else
                            break;
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"An error occurred while creating the database: {ex.Message}";
            }
        }

        protected string CreateConnectionString(bool trustedConnection, string serverName, string databaseName,
            string userName, string password, int timeout = 0)
        {
            var builder = new SqlConnectionStringBuilder
            {
                IntegratedSecurity = trustedConnection,
                DataSource = serverName,
                InitialCatalog = databaseName
            };

            if (!trustedConnection)
            {
                builder.UserID = userName;
                builder.Password = password;
            }

            builder.PersistSecurityInfo = false;

            if (this.UseMars)
            {
                builder.MultipleActiveResultSets = true;
            }

            if (timeout > 0)
            {
                builder.ConnectTimeout = timeout;
            }

            return builder.ConnectionString;
        }

        #endregion

        public IActionResult Index()
        {
            if (DataSettingsManager.DatabaseIsInstalled)
                return RedirectToRoute("HomePage");

            var model = new InstallModel
            {
                InstallSampleData = false,
                DatabaseConnectionString = "",
                DataProvider = DataProviderType.SqlServer,
                DisableSampleDataOption = _config.DisableSampleDataDuringInstallation,
                SqlAuthenticationType = "sqlauthentication",
                SqlConnectionInfo = "sqlconnectioninfo_values",
                SqlServerCreateDatabase = false,
                UseCustomCollation = false,
                Collation = "SQL_Latin1_General_CP1_CI_AS",
            };

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Index(InstallModel model)
        {
            if (DataSettingsManager.DatabaseIsInstalled)
                return RedirectToRoute("HomePage");

            if (model.DatabaseConnectionString != null)
                model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();

            model.DisableSampleDataOption = _config.DisableSampleDataDuringInstallation;

            if (model.DataProvider == DataProviderType.SqlServer)
            {
                if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw", StringComparison.InvariantCultureIgnoreCase))
                {
                    //raw connection string
                    if (string.IsNullOrEmpty(model.DatabaseConnectionString))
                        ModelState.AddModelError("", "A SQL connection string is required");

                    try
                    {
                        new SqlConnectionStringBuilder(model.DatabaseConnectionString);
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Wrong SQL connection string format");
                    }
                }
                else
                {
                    //values
                    if (string.IsNullOrEmpty(model.SqlServerName))
                        ModelState.AddModelError("", "SQL Server name is required");
                    if (string.IsNullOrEmpty(model.SqlDatabaseName))
                        ModelState.AddModelError("", "Database name is required");

                    //authentication type
                    if (model.SqlAuthenticationType.Equals("sqlauthentication", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //SQL authentication
                        if (string.IsNullOrEmpty(model.SqlServerUsername))
                            ModelState.AddModelError("", "SQL Username is required");
                        if (string.IsNullOrEmpty(model.SqlServerPassword))
                            ModelState.AddModelError("", "SQL Password is required");
                    }
                }
            }

            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            //validate permissions
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite();
            foreach (var dir in dirsToCheck)
                if (!FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                    ModelState.AddModelError("", $"The '{WindowsIdentity.GetCurrent().Name}' account is not granted with Modify permission on folder '{dir}'. Please configure these permissions.");

            var filesToCheck = FilePermissionHelper.GetFilesWrite();
            foreach (var file in filesToCheck)
                if (!FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                    ModelState.AddModelError("", $"The '{WindowsIdentity.GetCurrent().Name}' account is not granted with Modify permission on file '{file}'. Please configure these permissions.");

            if (ModelState.IsValid)
            {
                try
                {
                    var connectionString = string.Empty;
                    if (model.DataProvider == DataProviderType.SqlServer)
                    {
                        if (model.SqlConnectionInfo.Equals("sqlconnectioninfo_raw", StringComparison.InvariantCultureIgnoreCase))
                        {
                            //raw connection string
                            var sqlStringBuilder = new SqlConnectionStringBuilder(model.DatabaseConnectionString);
                            if (UseMars)
                            {
                                sqlStringBuilder.MultipleActiveResultSets = true;
                            }
                            connectionString = sqlStringBuilder.ToString();
                        }
                        else
                        {
                            //values
                            connectionString = CreateConnectionString(
                                model.SqlAuthenticationType == "windowsauthentication",
                                model.SqlServerName, model.SqlDatabaseName,
                                model.SqlServerUsername, model.SqlServerPassword);
                        }

                        if (model.SqlServerCreateDatabase)
                        {
                            if (!SqlServerDatabaseExists(connectionString))
                            {
                                var collation = model.UseCustomCollation ? model.Collation : "";

                                var errorCreatingDatabase = CreateDatabase(connectionString, collation);
                                if (!string.IsNullOrEmpty(errorCreatingDatabase))
                                    throw new Exception(errorCreatingDatabase);
                            }
                        }
                        else
                        {
                            if (!SqlServerDatabaseExists(connectionString))
                                throw new Exception("Database does not exist or you don't have permissions to connect to it");
                        }
                    }

                    DataSettingsManager.SaveSettings(new DataSettings
                    {
                        DataProvider = model.DataProvider,
                        DataConnectionString = connectionString
                    }, _fileProvider);

                    //initialize database
                    EngineContext.Current.Resolve<IDataProvider>().InitializeDatabase();

                    var installationService = EngineContext.Current.Resolve<IInstallationService>();
                    installationService.InstallData(model.AdminEmail, model.AdminUsername, model.AdminPassword, model.InstallSampleData);

                    DataSettingsManager.ResetCache();

                    //prepare plugins to install
                    var pluginService = EngineContext.Current.Resolve<IPluginService>();
                    pluginService.ClearInstalledPluginsList();

                    var pluginsIgnoredDuringInstallation = new List<string>();
                    if (!string.IsNullOrEmpty(_config.PluginsIgnoredDuringInstallation))
                    {
                        pluginsIgnoredDuringInstallation = _config.PluginsIgnoredDuringInstallation
                            .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(pluginName => pluginName.Trim())
                            .ToList();
                    }

                    var plugins = pluginService.GetPluginDescriptors<IPlugin>(LoadPluginsMode.All)
                        .Where(pluginDescriptor =>
                            !pluginsIgnoredDuringInstallation.Contains(pluginDescriptor.SystemName))
                        .OrderBy(pluginDescriptor => pluginDescriptor.Group).ToList();

                    foreach (var plugin in plugins)
                    {
                        pluginService.PreparePluginToInstall(plugin.SystemName);
                    }

                    //register default permissions
                    var permissionProviders = new List<Type> {typeof(StandardPermissionProvider)};
                    foreach (var providerType in permissionProviders)
                    {
                        var provider = (IPermissionProvider) Activator.CreateInstance(providerType);
                        EngineContext.Current.Resolve<IPermissionService>().InstallPermissionsAsync(provider)
                            .GetAwaiter().GetResult();
                    }

                    webHelper.RestartAppDomain();

                    //Redirect to home page
                    return RedirectToRoute("HomePage");
                }
                catch (Exception exception)
                {
                    //reset cache
                    DataSettingsManager.ResetCache();

                    var cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
                    cacheManager.Clear();

                    //clear provider settings if something got wrong
                    DataSettingsManager.SaveSettings(new DataSettings(), _fileProvider);

                    ModelState.AddModelError("", $"Setup failed: {exception.Message}");
                }
            }

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult RestartInstall()
        {
            if (DataSettingsManager.DatabaseIsInstalled)
                return RedirectToRoute("HomePage");

            //restart application
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            webHelper.RestartAppDomain();

            //Redirect to home page
            return RedirectToRoute("HomePage");
        }
    }
}