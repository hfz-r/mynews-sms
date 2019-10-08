using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Plugins;
using StockManagementSystem.LicenseManager.Data;
using StockManagementSystem.LicenseManager.Services;
using StockManagementSystem.Services.License;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Menu;

namespace StockManagementSystem.LicenseManager
{
    public class LicenseManager : BasePlugin, IAdminMenuPlugin, ILicenseManager
    {
        private readonly LicenseObjectContext _objectContext;
        private readonly IPermissionService _permissionService;
        private readonly ILicenseService _licenseService;
        private readonly IWebHelper _webHelper;

        public LicenseManager(
            LicenseObjectContext objectContext,
            IPermissionService permissionService, 
            ILicenseService licenseService,
            IWebHelper webHelper)
        {
            _objectContext = objectContext;
            _permissionService = permissionService;
            _licenseService = licenseService;
            _webHelper = webHelper;
        }

        private static IList<string> LicenseStateList => SingletonList<string>.Instance;

        public override void Install()
        {
            _objectContext.Install();

            base.Install();
        }

        public override void Uninstall()
        {
           _objectContext.Uninstall();

            base.Uninstall();
        }

        /// <summary>
        /// Setting menu
        /// </summary>
        /// <param name="rootNode">Current menu node</param>
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var isPermit = _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins).GetAwaiter().GetResult();

            var pluginMainMenu = new SiteMapNode
            {
                Title = "License Manager",
                Url = _webHelper.GetLocation() + "License/Configure",
                Visible = isPermit,
                SystemName = "license.mgr",
                IconClass = "fa fa-key"
            };

            var settingNode = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Configuration"));
            settingNode?.ChildNodes.Add(pluginMainMenu);
        }

        public async Task<string> GetPublicKey(string licenseToName, string licenseToEmail)
        {
            var license = (await _licenseService.GetAllLicenses())
                .FirstOrDefault(l => l.LicenseToName == licenseToName && l.LicenseToEmail == licenseToEmail);
            if (license == null)
                throw new ArgumentNullException(nameof(license));

            return license.PublicKey;
        }

        public async Task SaveLicenseState(string serialNo)
        {
            LicenseStateList.Add(serialNo);
            await Task.CompletedTask;
        }

        public async Task ResetLicenseInstance()
        {
            LicenseStateList.Clear();
            await Task.CompletedTask;
        }
    }
}