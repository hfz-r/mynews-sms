using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Core.Plugins
{
    /// <summary>
    /// Represents an information about plugins
    /// </summary>
    public class PluginsInfo
    {
        public void Save(IFileProviderHelper fileProvider)
        {
            var filePath = fileProvider.MapPath(PluginDefaults.PluginsInfoFilePath);
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        public static PluginsInfo LoadPluginInfo(IFileProviderHelper fileProvider)
        {
            var filePath = fileProvider.MapPath(PluginDefaults.PluginsInfoFilePath);
            if (!fileProvider.FileExists(filePath))
                throw new FileNotFoundException($"{filePath} is not in {PluginDefaults.PluginsInfoFilePath}");

            var text = fileProvider.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new PluginsInfo();

            return JsonConvert.DeserializeObject<PluginsInfo>(text);
        }

        #region Properties

        public IList<string> InstalledPluginNames { get; set; } = new List<string>();

        public IList<string> PluginNamesToUninstall { get; set; } = new List<string>();

        public IList<string> PluginNamesToDelete { get; set; } = new List<string>();

        public IList<(string SystemName, Guid? UserGuid)> PluginNamesToInstall { get; set; } = new List<(string SystemName, Guid? UserGuid)>();

        [JsonIgnore]
        public IList<string> IncompatiblePlugins { get; set; }

        [JsonIgnore]
        public IList<PluginLoadedAssemblyInfo> AssemblyLoadedCollision { get; set; }

        [JsonIgnore]
        public IEnumerable<PluginDescriptor> PluginDescriptors { get; set; }

        #endregion

    }
}