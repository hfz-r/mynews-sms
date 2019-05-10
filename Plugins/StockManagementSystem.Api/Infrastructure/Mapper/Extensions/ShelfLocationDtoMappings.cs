using StockManagementSystem.Api.DTOs.Generics;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class ShelfLocationDtoMappings
    {
        public static ShelfLocationDto ToDto(this ShelfLocation shelfLocation)
        {
            return shelfLocation.MapTo<ShelfLocation, ShelfLocationDto>();
        }
    }
}