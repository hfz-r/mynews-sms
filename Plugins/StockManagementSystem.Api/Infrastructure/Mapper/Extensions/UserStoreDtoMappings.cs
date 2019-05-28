using StockManagementSystem.Api.DTOs.Stores;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class UserStoreDtoMappings
    {
        public static UserStoreDto ToDto(this UserStore userStore)
        {
            return userStore.MapTo<UserStore, UserStoreDto>();
        }
    }
}