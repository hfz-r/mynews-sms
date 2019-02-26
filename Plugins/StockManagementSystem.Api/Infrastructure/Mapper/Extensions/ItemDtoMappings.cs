using StockManagementSystem.Api.DTOs.Items;
using StockManagementSystem.Core.Domain.Items;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class ItemDtoMappings
    {
        public static ItemDto ToDto(this Item item)
        {
            return item.MapTo<Item, ItemDto>();
        }
    }
}