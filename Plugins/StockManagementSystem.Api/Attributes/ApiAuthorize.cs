using Microsoft.AspNetCore.Authorization;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Plugins;

namespace StockManagementSystem.Api.Attributes
{
    public class ApiAuthorize : AuthorizeAttribute
    {
        public new string Policy
        {
            get => base.AuthenticationSchemes;
            set => base.AuthenticationSchemes = GetAuthenticationSchemeName(value);
        }

        public new string AuthenticationSchemes
        {
            get => base.AuthenticationSchemes;
            set => base.AuthenticationSchemes = GetAuthenticationSchemeName(value);
        }

        private static string GetAuthenticationSchemeName(string value)
        {
            var plugins = EngineContext.Current.Resolve<IPluginService>();
            var findPlugin = plugins.FindPluginByTypeInAssembly(typeof(ApiStartup));
            var pluginInstalled = findPlugin.PluginDescriptor?.Installed ?? false;

            return pluginInstalled ? value : default(string);
        }
    }
}