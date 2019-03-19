using System.Collections.Generic;
using StockManagementSystem.Core.Configuration;

namespace StockManagementSystem.Core.Domain.Security
{
    public class SecuritySettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether all pages will be forced to use SSL (no matter of a specified [HttpsRequirementAttribute] attribute)
        /// </summary>
        public bool ForceSslForAllPages { get; set; }

        /// <summary>
        /// Gets or sets an encryption key
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets a list of allowed IP addresses
        /// </summary>
        public List<string> AllowedIpAddresses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether XSRF protection should be enabled
        /// </summary>
        public bool EnableXsrfProtection { get; set; }
    }
}