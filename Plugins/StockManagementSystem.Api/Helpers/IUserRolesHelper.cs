using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Helpers
{
    public interface IUserRolesHelper
    {
        IList<Role> GetValidRoles(List<int> roleIds);

        bool IsInGuestsRole(IList<Role> roles);

        bool IsInRegisteredRole(IList<Role> roles);
    }
}