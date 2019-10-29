using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using StockManagementSystem.Api.Domain;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Plugins;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Menu;

namespace StockManagementSystem.Api
{
    public class ApiPlugin : BasePlugin, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;

        public ApiPlugin(ISettingService settingService, IPermissionService permissionService, IWebHelper webHelper)
        {
            _settingService = settingService;
            _permissionService = permissionService;
            _webHelper = webHelper;
        }

        public override void Install()
        {
            var settings = new ApiSettings {EnableApi = true};

            _settingService.SaveSetting(settings);

            base.Install();
        }

        public override void Uninstall()
        {
            var persistedGrantMigrator = EngineContext.Current.Resolve<PersistedGrantDbContext>().GetService<IMigrator>();
            persistedGrantMigrator.Migrate("0");

            var configurationMigrator = EngineContext.Current.Resolve<ConfigurationDbContext>().GetService<IMigrator>();
            configurationMigrator.Migrate("0");

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var canManagePlugin = _permissionService.Authorize(StandardPermissionProvider.ManagePlugins);

            var pluginMainMenu = new SiteMapNode
            {
                Title = "API Settings",
                Url = _webHelper.GetLocation() + "ApiSettings/Index",
                Visible = canManagePlugin,
                SystemName = "api.settings",
                IconClass = "glyphicon glyphicon-send"
            };

            var settingNode = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Configuration"));
            settingNode?.ChildNodes.Add(pluginMainMenu);
        }
    }
}