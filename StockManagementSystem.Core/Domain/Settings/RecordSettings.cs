using StockManagementSystem.Core.Configuration;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class RecordSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to ignore "limit per tenant" rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreTenantLimitations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore "limit per store" rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreStoreLimitations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore ACL rules (side-wide). It can significantly improve performance when enabled.
        /// </summary>
        public bool IgnoreAcl { get; set; }
    }
}