using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Core.Plugins
{
    /// <summary>
    /// Represents a plugin descriptor
    /// </summary>
    public class PluginDescriptor : IDescriptor, IComparable<PluginDescriptor>
    {
        public PluginDescriptor()
        {
            LimitedToUserRoles = new List<int>();
        }

        public PluginDescriptor(Assembly referencedAssembly) : this()
        {
            this.ReferencedAssembly = referencedAssembly;
        }

        public virtual TPlugin Instance<TPlugin>() where TPlugin : class, IPlugin
        {
            //try to resolve plugin as unregistered service
            var instance = EngineContext.Current.ResolveUnregistered(PluginType);

            //try to get typed instance
            var typedInstance = instance as TPlugin;
            if (typedInstance != null)
                typedInstance.PluginDescriptor = this;

            return typedInstance;
        }

        public virtual void Save()
        {
            var fileProvider = EngineContext.Current.Resolve<IFileProviderHelper>();

            if (OriginalAssemblyFile == null)
                throw new Exception($"Cannot load original assembly path for {SystemName} plugin.");

            var filePath = fileProvider.Combine(fileProvider.GetDirectoryName(OriginalAssemblyFile), PluginDefaults.DescriptionFileName);
            if (!fileProvider.FileExists(filePath))
                throw new Exception($"Description file for {SystemName} plugin does not exist. {filePath}");

            //save the file
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        public static PluginDescriptor GetPluginDescriptorFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new PluginDescriptor();

            //get plugin descriptor from the JSON file
            var descriptor = JsonConvert.DeserializeObject<PluginDescriptor>(text);

            return descriptor;
        }

        public int CompareTo(PluginDescriptor other)
        {
            return string.Compare(FriendlyName, other.FriendlyName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        public override bool Equals(object value)
        {
            return SystemName?.Equals((value as PluginDescriptor)?.SystemName) ?? false;
        }

        public override int GetHashCode()
        {
            return SystemName.GetHashCode();
        }

        #region Properties

        [JsonProperty(PropertyName = "Group")]
        public virtual string Group { get; set; }

        [JsonProperty(PropertyName = "FriendlyName")]
        public virtual string FriendlyName { get; set; }

        [JsonProperty(PropertyName = "SystemName")]
        public virtual string SystemName { get; set; }

        [JsonProperty(PropertyName = "Version")]
        public virtual string Version { get; set; }

        [JsonProperty(PropertyName = "Author")]
        public virtual string Author { get; set; }

        [JsonProperty(PropertyName = "FileName")]
        public virtual string AssemblyFileName { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public virtual string Description { get; set; }

        [JsonProperty(PropertyName = "LimitedToUserRoles")]
        public virtual IList<int> LimitedToUserRoles { get; set; }

        [JsonIgnore]
        public virtual bool Installed { get; set; }

        [JsonIgnore]
        public virtual Type PluginType { get; set; }

        [JsonIgnore]
        public virtual string OriginalAssemblyFile { get; internal set; }

        [JsonIgnore]
        public virtual Assembly ReferencedAssembly { get; internal set; }

        [JsonIgnore]
        public virtual bool ShowInPluginsList { get; set; } = true;

        #endregion
    }
}