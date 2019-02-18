using System;
using System.Collections.Generic;
using System.Linq;

namespace StockManagementSystem.Core.Plugins
{
    /// <summary>
    /// Represents an information about assembly which loaded by plugins
    /// </summary>
    public class PluginLoadedAssemblyInfo
    {
        public PluginLoadedAssemblyInfo(string shortName, string assemblyInMemory)
        {
            this.ShortName = shortName;
            this.References = new List<(string PluginName, string AssemblyName)>();
            this.AssemblyFullNameInMemory = assemblyInMemory;
        }

        public string ShortName { get; }

        public string AssemblyFullNameInMemory { get; }

        public List<(string PluginName, string AssemblyName)> References { get; }

        public IList<(string PluginName, string AssemblyName)> Collisions => References.Where(reference =>
                !reference.AssemblyName.Equals(AssemblyFullNameInMemory, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
    }
}