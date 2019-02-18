using StockManagementSystem.Core.Plugins;

namespace StockManagementSystem.Web.Menu
{
    /// <summary>
    /// Interface for plugins which have some items in the admin area menu
    /// </summary>
    public interface IAdminMenuPlugin : IPlugin
    {
        void ManageSiteMap(SiteMapNode rootNode);
    }
}