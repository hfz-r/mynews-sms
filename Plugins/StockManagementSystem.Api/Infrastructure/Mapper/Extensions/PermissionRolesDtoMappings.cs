using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class PermissionRolesDtoMappings
    {
        public static PermissionRolesDto ToDto(this PermissionRoles permissionRoles)
        {
            return permissionRoles.MapTo<PermissionRoles, PermissionRolesDto>();
        }
    }
}