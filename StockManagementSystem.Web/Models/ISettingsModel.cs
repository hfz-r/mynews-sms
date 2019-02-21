namespace StockManagementSystem.Web.Models
{
    /// <summary>
    /// Represents a settings model
    /// </summary>
    public interface ISettingsModel
    {
        /// <summary>
        /// Gets or sets an active tenant scope configuration (tenant identifier)
        /// </summary>
        int ActiveTenantScopeConfiguration { get; set; }
    }
}