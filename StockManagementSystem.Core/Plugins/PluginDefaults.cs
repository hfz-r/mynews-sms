namespace StockManagementSystem.Core.Plugins
{
    /// <summary>
    /// Represents default values related to plugins
    /// </summary>
    public static class PluginDefaults
    {
        public static string InstalledPluginsFilePath => "~/App_Data/installedPlugins.json";

        public static string PluginsInfoFilePath => "~/App_Data/plugins.json";

        public static string Path => "~/Plugins";

        public static string PathName => "Plugins";

        public static string ShadowCopyPath => "~/Plugins/bin";

        public static string RefsPathName => "refs";

        public static string DescriptionFileName => "plugin.json";

        public static string ReserveShadowCopyPathName => "reserve_bin_";

        public static string ReserveShadowCopyPathNamePattern => "reserve_bin_*";
    }
}