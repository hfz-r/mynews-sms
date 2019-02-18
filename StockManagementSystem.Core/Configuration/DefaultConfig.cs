namespace StockManagementSystem.Core.Configuration
{
    /// <summary>
    /// Represents startup Default configuration parameters
    /// </summary>
    public class DefaultConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display the full error in production environment.
        /// It's ignored (always enabled) in development environment
        /// </summary>
        public bool DisplayFullErrorStack { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether can install sample data during installation
        /// </summary>
        public bool DisableSampleDataDuringInstallation { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to use fast installation. 
        /// By default this setting should always be set to "False" (only for advanced users)
        /// </summary>
        public bool UseFastInstallationService { get; set; }
        /// <summary>
        /// Gets or sets a list of plugins ignored during installation
        /// </summary>
        public string PluginsIgnoredDuringInstallation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to clear /Plugins/bin directory on application startup
        /// </summary>
        public bool ClearPluginShadowDirectoryOnStartup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to copy "locked" assemblies from /Plugins/bin directory to temporary subdirectories on application startup
        /// </summary>
        public bool CopyLockedPluginAssembilesToSubdirectoriesOnStartup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to load an assembly into the load-from context, bypassing some security checks.
        /// </summary>
        public bool UseUnsafeLoadAssembly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to copy plugins library to the /Plugins/bin directory on application startup
        /// </summary>
        public bool UsePluginsShadowCopy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use backwards compatibility with SQL Server 2008 and SQL Server 2008R2
        /// </summary>
        public bool UseRowNumberForPaging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to store TempData in the session state.
        /// By default the cookie-based TempData provider is used to store TempData in cookies.
        /// </summary>
        public bool UseSessionStateTempDataProvider { get; set; }
    }
}