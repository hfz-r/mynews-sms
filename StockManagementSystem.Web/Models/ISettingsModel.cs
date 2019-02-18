namespace StockManagementSystem.Web.Models
{
    /// <summary>
    /// Represents a settings model
    /// </summary>
    public interface ISettingsModel
    {
        /// <summary>
        /// Gets or sets an active store scope configuration (store identifier)
        /// </summary>
        int ActiveStoreScopeConfiguration { get; set; }
    }
}