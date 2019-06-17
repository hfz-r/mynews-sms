using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Infrastructure.Cache;
using StockManagementSystem.Models.Common;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Media;
using StockManagementSystem.Services.Plugins;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Security;
using StockManagementSystem.Web.UI;

namespace StockManagementSystem.Factories
{
    public class CommonModelFactory : ICommonModelFactory
    {
        private readonly CommonSettings _commonSettings;
        private readonly UserSettings _userSettings;
        private readonly RecordSettings _recordSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IFileProviderHelper _fileProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IUserService _userService;
        private readonly IStoreService _storeService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ITenantContext _tenantContext;
        private readonly IPluginService _pluginService;

        public CommonModelFactory(
            CommonSettings commonSettings,
            UserSettings userSettings,
            RecordSettings recordSettings,
            IGenericAttributeService genericAttributeService,
            IStaticCacheManager cacheManager,
            IFileProviderHelper fileProvider,
            IDateTimeHelper dateTimeHelper,
            IHttpContextAccessor httpContextAccessor,
            IPageHeadBuilder pageHeadBuilder,
            IPermissionService permissionService,
            IPictureService pictureService,
            IUserService userService,
            IStoreService storeService,
            IWebHelper webHelper,
            IWorkContext workContext,
            ITenantContext tenantContext,
            IPluginService pluginService)
        {
            _commonSettings = commonSettings;
            _userSettings = userSettings;
            _recordSettings = recordSettings;
            _genericAttributeService = genericAttributeService;
            _cacheManager = cacheManager;
            _fileProvider = fileProvider;
            _dateTimeHelper = dateTimeHelper;
            _httpContextAccessor = httpContextAccessor;
            _pageHeadBuilder = pageHeadBuilder;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _userService = userService;
            _storeService = storeService;
            _webHelper = webHelper;
            _workContext = workContext;
            _tenantContext = tenantContext;
            _pluginService = pluginService;
        }

        #region Utilities

        protected Task PrepareTenantUrlWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether current tenant URL matches the tenant`s configured URL
            var currentTenantUrl = _tenantContext.CurrentTenant.Url;
            if (!string.IsNullOrEmpty(currentTenantUrl) &&
                (currentTenantUrl.Equals(_webHelper.GetLocation(false), StringComparison.InvariantCultureIgnoreCase) ||
                 currentTenantUrl.Equals(_webHelper.GetLocation(true), StringComparison.InvariantCultureIgnoreCase)))
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = "Specified tenant URL matches this tenant URL"
                });
                return Task.CompletedTask;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = $"Specified tenant URL ({currentTenantUrl}) doesn't match this tenant URL ({_webHelper.GetLocation(false)})"
            });

            return Task.CompletedTask;
        }

        protected Task PreparePluginsWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether there are incompatible plugins
            foreach (var pluginName in _pluginService.GetIncompatiblePlugins())
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = $"'{pluginName}' plugin is not compatible or cannot be loaded."
                });
            }

            //check whether there are any collision of loaded assembly
            foreach (var assembly in _pluginService.GetAssemblyCollisions())
            {
                //get plugin references message
                var message = assembly.Collisions
                    .Select(item => $"the '{item.PluginName}' plugin required the '{item.AssemblyName}' assembly")
                    .Aggregate("", (current, all) => all + ", " + current).TrimEnd(',', ' ');

                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = $"The '{assembly.ShortName}' assembly has collision, application loaded the '{assembly.AssemblyFullNameInMemory}' assembly, but {message}"
                });
            }

            return Task.CompletedTask;
        }

        protected async Task PreparePerformanceSettingsWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //check whether "IgnoreStoreLimitations" setting disabled
            if (!_recordSettings.IgnoreStoreLimitations && (await _storeService.GetAllStores()).Count == 1)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Recommendation,
                    Text = "Performance. You use only one store. Recommended to ignore store limitations (record settings)"
                });
            }

            //check whether "IgnoreAcl" setting disabled
            if (!_recordSettings.IgnoreAcl)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Recommendation,
                    Text = "Performance. Recommended to ignore ACL rules if you don't use them (record settings)"
                });
            }
        }

        protected Task PrepareFilePermissionsWarningModel(IList<SystemWarningModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            var dirPermissionsOk = true;
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite();
            foreach (var dir in dirsToCheck)
            {
                if (FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                    continue;

                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = $"The '{WindowsIdentity.GetCurrent().Name}' account is not granted with Modify permission on folder '{dir}'. Please configure these permissions."
                });
                dirPermissionsOk = false;
            }

            if (dirPermissionsOk)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = "All directory permissions are OK"
                });
            }

            var filePermissionsOk = true;
            var filesToCheck = FilePermissionHelper.GetFilesWrite();
            foreach (var file in filesToCheck)
            {
                if (FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                    continue;

                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = $"The '{WindowsIdentity.GetCurrent().Name}' account is not granted with Modify permission on file '{file}'. Please configure these permissions."
                });
                filePermissionsOk = false;
            }

            if (filePermissionsOk)
            {
                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = "All file permissions are OK"
                });
            }

            return Task.CompletedTask;
        }
        
        #endregion

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>Admin header links model</returns>
        public async Task<AdminHeaderLinksModel> PrepareAdminHeaderLinksModel()
        {
            var model = new AdminHeaderLinksModel
            {
                DisplayAdminLink = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessPanel),
                EditPageUrl = _pageHeadBuilder.GetEditPageUrl()
            };

            return model;
        }

        /// <summary>
        /// Prepare the header links model
        /// </summary>
        /// <returns>Header links model</returns>
        public async Task<HeaderLinksModel> PrepareHeaderLinksModel()
        {
            var user = _workContext.CurrentUser;

            var model = new HeaderLinksModel
            {
                IsAuthenticated = user.IsRegistered(),
                UserName = user.IsRegistered() ? await _userService.GetUserFullNameAsync(user) : "",
            };

            return model;
        }

        /// <summary>
        /// Prepare the logo model
        /// </summary>
        /// <returns>Logo model</returns>
        public Task<LogoModel> PrepareLogoModel()
        {
            var model = new LogoModel {TenantName = _tenantContext.CurrentTenant.Name};

            var cacheKey = string.Format(ModelCacheDefaults.LogoPath, _tenantContext.CurrentTenant.Id, _webHelper.IsCurrentConnectionSecured());
            model.LogoPath = _cacheManager.Get(cacheKey, () =>
            {
                var logo = "";
                var logoPictureId = _commonSettings.LogoPictureId;
                if (logoPictureId > 0)
                    logo = _pictureService.GetPictureUrl(logoPictureId, showDefaultPicture: false);

                if (string.IsNullOrEmpty(logo))
                    //use default logo
                    logo = $"{_webHelper.GetLocation()}images/MainMenu_Logo.png";

                return logo;
            });

            return Task.FromResult(model);
        }

        public Task<SystemInfoModel> PrepareSystemInfoModel(SystemInfoModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Version = Core.Version.CurrentVersion;
            model.ServerTimeZone = TimeZoneInfo.Local.StandardName;
            model.ServerLocalTime = DateTime.Now;
            model.UtcTime = DateTime.UtcNow;
            model.CurrentUserTime = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            model.HttpHost = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];

            //ensure no exception is thrown
            try
            {
                model.OperatingSystem = Environment.OSVersion.VersionString;
                model.AspNetInfo = RuntimeEnvironment.GetSystemVersion();
                model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted.ToString();
            }
            catch
            {
                // ignored
            }

            foreach (var header in _httpContextAccessor.HttpContext.Request.Headers)
            {
                model.Headers.Add(new SystemInfoModel.HeaderModel
                {
                    Name = header.Key,
                    Value = header.Value
                });
            }

            return Task.FromResult(model);
        }

        public async Task<IList<SystemWarningModel>> PrepareSystemWarningModels()
        {
            var models = new List<SystemWarningModel>();

            //tenant URL
            await PrepareTenantUrlWarningModel(models);

            //incompatible plugins //TODO: control pages maybe?
            //await PreparePluginsWarningModel(models);

            //performance settings
            await PreparePerformanceSettingsWarningModel(models);

            //validate write permissions (the same procedure like during installation)
            await PrepareFilePermissionsWarningModel(models);

            return models;
        }

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        public virtual string PrepareRobotsTextFile()
        {
            var sb = new StringBuilder();

            //if robots.custom.txt exists, let's use it instead of hard-coded data below
            var robotsFilePath = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.custom.txt");
            if (_fileProvider.FileExists(robotsFilePath))
            {
                var robotsFileContent = _fileProvider.ReadAllText(robotsFilePath, Encoding.UTF8);
                sb.Append(robotsFileContent);
            }
            else
            {
                //doesn't exist. Let's generate it (default behavior)

                var disallowPaths = new List<string>
                {
                    "/bin/",
                    "/files/",
                    "/install",
                    "/account/avatar",
                    "/account/changepassword",
                    "/account/info",
                    "/checkusernameavailability",
                    "/forgotpassword/confirm",
                };

                const string newLine = "\r\n"; //Environment.NewLine
                sb.Append("User-agent: *");
                sb.Append(newLine);
                //host
                sb.AppendFormat("Host: {0}", _webHelper.GetLocation());
                sb.Append(newLine);
                //allow-paths
                sb.Append("Allow: WebSurge");
                sb.Append(newLine);
                //disallow-paths
                foreach (var path in disallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }

                //load and add robots.txt additions to the end of file.
                var robotsAdditionsFile = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.additions.txt");
                if (_fileProvider.FileExists(robotsAdditionsFile))
                {
                    var robotsFileContent = _fileProvider.ReadAllText(robotsAdditionsFile, Encoding.UTF8);
                    sb.Append(robotsFileContent);
                }
            }

            return sb.ToString();
        }
    }
}