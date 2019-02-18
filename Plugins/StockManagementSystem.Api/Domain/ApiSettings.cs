using StockManagementSystem.Core.Configuration;

namespace StockManagementSystem.Api.Domain
{
    public class ApiSettings : ISettings
    {
        public bool EnableApi { get; set; }
        public bool EnableLogging { get; set; }
    }
}