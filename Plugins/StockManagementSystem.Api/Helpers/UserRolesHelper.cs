using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Helpers
{
    public class UserRolesHelper : IUserRolesHelper
    {
        private const string RolesAllKey = "userrole.all-{0}";

        private readonly IUserService _userService;
        private readonly ICacheManager _cacheManager;

        public UserRolesHelper(IUserService userService, ICacheManager cacheManager)
        {
            _userService = userService;
            _cacheManager = cacheManager;
        }

        public IList<Role> GetValidRoles(List<int> roleIds)
        {
            _cacheManager.RemoveByPattern(RolesAllKey);

            var roles = _userService.GetRoles(true);

            return roles.Where(role => roleIds != null && roleIds.Contains(role.Id)).ToList();
        }

        public bool IsInGuestsRole(IList<Role> roles)
        {
            return roles.FirstOrDefault(r => r.SystemName == UserDefaults.GuestsRoleName) != null;
        }

        public bool IsInRegisteredRole(IList<Role> roles)
        {
            return roles.FirstOrDefault(r => r.SystemName == UserDefaults.RegisteredRoleName) != null;
        }
    }
}