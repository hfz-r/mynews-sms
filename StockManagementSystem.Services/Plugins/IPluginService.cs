using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Plugins;

namespace StockManagementSystem.Services.Plugins
{
    public interface IPluginService
    {
        void ClearInstalledPluginsList();
        Task DeletePlugins();
        IPlugin FindPluginByTypeInAssembly(Type typeInAssembly);
        IList<PluginLoadedAssemblyInfo> GetAssemblyCollisions();
        IList<string> GetIncompatiblePlugins();

        PluginDescriptor GetPluginDescriptorBySystemName<TPlugin>(string systemName,
            LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly, User user = null, string group = null)
            where TPlugin : class, IPlugin;

        IEnumerable<PluginDescriptor> GetPluginDescriptors<TPlugin>(
            LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly, User user = null, string group = null)
            where TPlugin : class, IPlugin;

        IEnumerable<TPlugin> GetPlugins<TPlugin>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            User user = null, string group = null) where TPlugin : class, IPlugin;

        Task InstallPlugins();
        bool IsRestartRequired();
        void PreparePluginToDelete(string systemName);
        void PreparePluginToInstall(string systemName, User user = null);
        void PreparePluginToUninstall(string systemName);
        void ResetChanges();
        Task UninstallPlugins();
    }
}