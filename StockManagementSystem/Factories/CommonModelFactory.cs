using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Infrastructure.Cache;
using StockManagementSystem.Models.Common;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Media;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.UI;

namespace StockManagementSystem.Factories
{
    public class CommonModelFactory : ICommonModelFactory
    {
        private readonly CommonSettings _commonSettings;
        private readonly UserSettings _userSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IFileProviderHelper _fileProvider;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IUserService _userService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ITenantContext _tenantContext;

        public CommonModelFactory(
            CommonSettings commonSettings,
            UserSettings userSettings,
            IGenericAttributeService genericAttributeService,
            IStaticCacheManager cacheManager,
            IFileProviderHelper fileProvider,
            IPageHeadBuilder pageHeadBuilder,
            IPermissionService permissionService,
            IPictureService pictureService,
            IUserService userService,
            IWebHelper webHelper,
            IWorkContext workContext,
            ITenantContext tenantContext)
        {
            _commonSettings = commonSettings;
            _userSettings = userSettings;
            _genericAttributeService = genericAttributeService;
            _cacheManager = cacheManager;
            _fileProvider = fileProvider;
            _pageHeadBuilder = pageHeadBuilder;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _userService = userService;
            _webHelper = webHelper;
            _workContext = workContext;
            _tenantContext = tenantContext;
        }

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
    }
}