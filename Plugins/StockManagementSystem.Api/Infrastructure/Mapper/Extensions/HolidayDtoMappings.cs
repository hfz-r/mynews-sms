using StockManagementSystem.Api.DTOs.Directory;
using StockManagementSystem.Core.Domain.Directory;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class HolidayDtoMappings
    {
        public static HolidayDto ToDto(this Holiday holiday)
        {
            return holiday.MapTo<Holiday, HolidayDto>();
        }
    }
}