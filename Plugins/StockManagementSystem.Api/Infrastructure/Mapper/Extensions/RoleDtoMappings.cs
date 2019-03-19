using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class RoleDtoMappings
    {
        public static RoleDto ToDto(this Role role)
        {
            return role.MapTo<Role, RoleDto>();
        }

        public static Role ToEntity(this RoleDto roleDto)
        {
            return roleDto.MapTo<RoleDto, Role>();
        }
    }
}