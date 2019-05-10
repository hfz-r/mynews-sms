using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class PermissionDtoMappings
    {
        public static PermissionDto ToDto(this Permission permission)
        {
            return permission.MapTo<Permission, PermissionDto>();
        }
    }
}