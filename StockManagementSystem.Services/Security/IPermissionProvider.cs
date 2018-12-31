using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Services.Security
{
    public interface IPermissionProvider
    {
        IEnumerable<DefaultPermission> GetDefaultPermissions();

        IEnumerable<Permission> GetPermissions();
    }
}