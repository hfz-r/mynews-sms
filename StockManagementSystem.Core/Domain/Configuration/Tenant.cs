namespace StockManagementSystem.Core.Domain.Configuration
{
    public class Tenant : BaseEntity
    {
        /// <summary>
        /// Gets or sets the tenant name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tenant URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled
        /// </summary>
        public bool SslEnabled { get; set; }

        /// <summary>
        /// Gets or sets the comma separated list of possible HTTP_HOST values
        /// </summary>
        public string Hosts { get; set; }
    }
}