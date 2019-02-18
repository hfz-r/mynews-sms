using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class UserDtoMappings
    {
        public static UserDto ToDto(this User user)
        {
            return user.MapTo<User, UserDto>();
        }
    }
}