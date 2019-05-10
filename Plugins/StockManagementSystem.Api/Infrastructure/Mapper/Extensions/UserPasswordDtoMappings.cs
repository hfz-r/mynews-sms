using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class UserPasswordDtoMappings
    {
        public static UserPasswordDto ToDto(this UserPassword userPassword)
        {
            return userPassword.MapTo<UserPassword, UserPasswordDto>();
        }

        public static UserPassword ToEntity(this UserPasswordDto userPasswordDto)
        {
            return userPasswordDto.MapTo<UserPasswordDto, UserPassword>();
        }
    }
}