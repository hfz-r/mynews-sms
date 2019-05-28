using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class UserRoleDtoMappings
    {
        public static UserRoleDto ToDto(this UserRole userRole)
        {
            return userRole.MapTo<UserRole, UserRoleDto>();
        }
    }
}