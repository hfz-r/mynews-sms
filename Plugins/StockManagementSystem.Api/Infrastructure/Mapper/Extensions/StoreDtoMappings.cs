using StockManagementSystem.Api.DTOs.Stores;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class StoreDtoMappings
    {
        public static StoreDto ToDto(this Store store)
        {
            return store.MapTo<Store, StoreDto>();
        }

        public static Store ToEntity(this StoreDto storeDto)
        {
            return storeDto.MapTo<StoreDto, Store>();
        }
    }
}