using StockManagementSystem.Core.Configuration;

namespace StockManagementSystem.Services.Helpers
{
    public class DateTimeSettings : ISettings
    {
        public string DefaultTimeZoneId { get; set; }

        public bool AllowUsersToSetTimeZone { get; set; }
    }
}