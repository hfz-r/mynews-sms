using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Plugins;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Services.Plugins
{
    public class PluginService : IPluginService
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private readonly IFileProviderHelper _fileProvider;
        private readonly PluginsInfo _pluginsInfo;

        public PluginService(
            IUserService userService,
            ILogger logger,
            IFileProviderHelper fileProvider)
        {
            _userService = userService;
            _logger = logger;
            _fileProvider = fileProvider;
            _pluginsInfo = Singleton<PluginsInfo>.Instance;
        }

        /// <summary>
        /// Check whether to load the plugin based on the load mode passed
        /// </summary>
        protected bool FilterByLoadMode(PluginDescriptor pluginDescriptor, LoadPluginsMode loadMode)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            switch (loadMode)
            {
                case LoadPluginsMode.All:
                    return true;

                case LoadPluginsMode.InstalledOnly:
                    return pluginDescriptor.Installed;

                case LoadPluginsMode.NotInstalledOnly:
                    return !pluginDescriptor.Installed;

                default:
                    throw new NotSupportedException(nameof(loadMode));
            }
        }

        /// <summary>
        /// Check whether to load the plugin based on the plugin group passed
        /// </summary>
        protected bool FilterByPluginGroup(PluginDescriptor pluginDescriptor, string group)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (string.IsNullOrEmpty(group))
                return true;

            return group.Equals(pluginDescriptor.Group, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check whether to load the plugin based on the user passed
        /// </summary>
        protected bool FilterByUser(PluginDescriptor pluginDescriptor, User user)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (user == null || !pluginDescriptor.LimitedToUserRoles.Any())
                return true;

            var roleIds = user.Roles.Where(role => role.Active).Select(r => r.Id);

            return pluginDescriptor.LimitedToUserRoles.Intersect(roleIds).Any();
        }

        public IEnumerable<PluginDescriptor> GetPluginDescriptors<TPlugin>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly, 
            User user = null, string group = null) where TPlugin : class, IPlugin
        {
            var pluginDescriptors = _pluginsInfo.PluginDescriptors;

            //filter plugins
            pluginDescriptors = pluginDescriptors.Where(descriptor =>
                FilterByLoadMode(descriptor, loadMode) &&
                FilterByUser(descriptor, user) &&
                FilterByPluginGroup(descriptor, group));

            //filter by the passed type
            if (typeof(TPlugin) != typeof(IPlugin))
                pluginDescriptors = pluginDescriptors.Where(descriptor => typeof(TPlugin).IsAssignableFrom(descriptor.PluginType));

            //order by group name
            pluginDescriptors = pluginDescriptors.OrderBy(descriptor => descriptor.Group).ToList();

            return pluginDescriptors;
        }

        public PluginDescriptor GetPluginDescriptorBySystemName<TPlugin>(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly, 
            User user = null, string group = null) where TPlugin : class, IPlugin
        {
            return GetPluginDescriptors<TPlugin>(loadMode, user, group)
                .FirstOrDefault(descriptor => descriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<TPlugin> GetPlugins<TPlugin>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly, 
            User user = null, string group = null) where TPlugin : class, IPlugin
        {
            return GetPluginDescriptors<TPlugin>(loadMode, user, group).Select(descriptor => descriptor.Instance<TPlugin>());
        }

        public IPlugin FindPluginByTypeInAssembly(Type typeInAssembly)
        {
            if (typeInAssembly == null)
                throw new ArgumentNullException(nameof(typeInAssembly));

            var pluginDescriptor = _pluginsInfo.PluginDescriptors.FirstOrDefault(descriptor =>
                descriptor.ReferencedAssembly?.FullName.Equals(typeInAssembly.Assembly.FullName,
                    StringComparison.InvariantCultureIgnoreCase) ?? false);

            return pluginDescriptor?.Instance<IPlugin>();
        }

        public void PreparePluginToInstall(string systemName, User user = null)
        {
            if (!_pluginsInfo.PluginNamesToInstall.Any(item => item.SystemName == systemName))
            {
                _pluginsInfo.PluginNamesToInstall.Add((systemName, user?.UserGuid));
                _pluginsInfo.Save(_fileProvider);
            }
        }

        public void PreparePluginToUninstall(string systemName)
        {
            if (!_pluginsInfo.PluginNamesToUninstall.Contains(systemName))
            {
                _pluginsInfo.PluginNamesToUninstall.Add(systemName);
                _pluginsInfo.Save(_fileProvider);
            }
        }

        public void PreparePluginToDelete(string systemName)
        {
            if (!_pluginsInfo.PluginNamesToDelete.Contains(systemName))
            {
                _pluginsInfo.PluginNamesToDelete.Add(systemName);
                _pluginsInfo.Save(_fileProvider);
            }
        }

        public void ResetChanges()
        {
            _pluginsInfo.PluginNamesToDelete.Clear();
            _pluginsInfo.PluginNamesToInstall.Clear();
            _pluginsInfo.PluginNamesToUninstall.Clear();
            _pluginsInfo.Save(_fileProvider);

            _pluginsInfo.PluginDescriptors.ToList().ForEach(pluginDescriptor => pluginDescriptor.ShowInPluginsList = true);
        }

        public void ClearInstalledPluginsList()
        {
            _pluginsInfo.InstalledPluginNames.Clear();
        }

        public async Task InstallPlugins()
        {
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => !descriptor.Installed);

            pluginDescriptors = pluginDescriptors.Where(descriptor => _pluginsInfo.PluginNamesToInstall
                .Any(item => item.SystemName.Equals(descriptor.SystemName))).ToList();
            if (!pluginDescriptors.Any())
                return;

            //do not inject services via constructor because it'll cause circular references
            var userActivityService = EngineContext.Current.Resolve<IUserActivityService>();

            foreach (var descriptor in pluginDescriptors)
            {
                try
                {
                    descriptor.Instance<IPlugin>().Install();

                    var pluginToInstall = _pluginsInfo.PluginNamesToInstall
                        .FirstOrDefault(plugin => plugin.SystemName.Equals(descriptor.SystemName));
                    _pluginsInfo.InstalledPluginNames.Add(descriptor.SystemName);
                    _pluginsInfo.PluginNamesToInstall.Remove(pluginToInstall);

                    var user = await _userService.GetUserByGuidAsync(pluginToInstall.UserGuid ?? Guid.Empty);
                    await userActivityService.InsertActivityAsync(user, "InstallNewPlugin", $"Installed a new plugin (FriendlyName: '{descriptor.SystemName}')");

                    //mark the plugin as installed
                    descriptor.Installed = true;
                    descriptor.ShowInPluginsList = true;
                }
                catch (Exception exception)
                {
                    _logger.Error($"The plugin {descriptor.SystemName} not installed", exception);
                }
            }

            _pluginsInfo.Save(_fileProvider);
        }

        public async Task UninstallPlugins()
        {
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => descriptor.Installed);

            pluginDescriptors = pluginDescriptors.Where(descriptor => _pluginsInfo.PluginNamesToUninstall.Contains(descriptor.SystemName)).ToList();
            if (!pluginDescriptors.Any())
                return;

            //do not inject services via constructor because it'll cause circular references
            var userActivityService = EngineContext.Current.Resolve<IUserActivityService>();

            foreach (var descriptor in pluginDescriptors)
            {
                try
                {
                    descriptor.Instance<IPlugin>().Uninstall();

                    _pluginsInfo.InstalledPluginNames.Remove(descriptor.SystemName);
                    _pluginsInfo.PluginNamesToUninstall.Remove(descriptor.SystemName);

                    await userActivityService.InsertActivityAsync("UninstallPlugin", $"Uninstalled a plugin (FriendlyName: '{descriptor.SystemName}')");

                    //mark the plugin as uninstalled
                    descriptor.Installed = false;
                    descriptor.ShowInPluginsList = true;
                }
                catch (Exception exception)
                {
                    _logger.Error($"The plugin {descriptor.SystemName} not uninstalled", exception);
                }
            }

            _pluginsInfo.Save(_fileProvider);
        }

        public async Task DeletePlugins()
        {
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => !descriptor.Installed);

            pluginDescriptors = pluginDescriptors.Where(descriptor => _pluginsInfo.PluginNamesToDelete.Contains(descriptor.SystemName)).ToList();
            if (!pluginDescriptors.Any())
                return;

            //do not inject services via constructor because it'll cause circular references
            var userActivityService = EngineContext.Current.Resolve<IUserActivityService>();

            foreach (var descriptor in pluginDescriptors)
            {
                try
                {
                    var pluginDirectory = _fileProvider.GetDirectoryName(descriptor.OriginalAssemblyFile);
                    if (_fileProvider.DirectoryExists(pluginDirectory))
                        _fileProvider.DeleteDirectory(pluginDirectory);

                    _pluginsInfo.PluginNamesToDelete.Remove(descriptor.SystemName);

                    await userActivityService.InsertActivityAsync("UninstallPlugin", $"Deleted a plugin (FriendlyName: '{descriptor.SystemName}')");
                }
                catch (Exception exception)
                {
                    _logger.Error($"The plugin {descriptor.SystemName} not deleted", exception);
                }
            }

            _pluginsInfo.Save(_fileProvider);
        }

        public bool IsRestartRequired()
        {
            return _pluginsInfo.PluginNamesToInstall.Any() || _pluginsInfo.PluginNamesToUninstall.Any() || _pluginsInfo.PluginNamesToDelete.Any();
        }

        public IList<string> GetIncompatiblePlugins()
        {
            return _pluginsInfo.IncompatiblePlugins;
        }

        public IList<PluginLoadedAssemblyInfo> GetAssemblyCollisions()
        {
            return _pluginsInfo.AssemblyLoadedCollision;
        }
    }
}