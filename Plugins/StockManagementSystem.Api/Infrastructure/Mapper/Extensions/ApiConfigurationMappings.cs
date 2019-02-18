using StockManagementSystem.Api.Domain;
using StockManagementSystem.Api.Models.ApiSettings;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class ApiConfigurationMappings
    {
        public static ApiConfigurationModel ToModel(this ApiSettings apiSettings)
        {
            return apiSettings.MapTo<ApiSettings, ApiConfigurationModel>();
        }

        public static ApiSettings ToEntity(this ApiConfigurationModel apiConfigurationModel)
        {
            return apiConfigurationModel.MapTo<ApiConfigurationModel, ApiSettings>();
        }
    }
}