using StockManagementSystem.Core.Configuration;

namespace StockManagementSystem.Services.Helpers
{
    public class DateTimeSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a default time zone identifier
        /// </summary>
        public string DefaultTimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to select theirs time zone
        /// </summary>
        public bool AllowUsersToSetTimeZone { get; set; }
    }
}