using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Models.Security;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Factories
{
    public class SecurityModelFactory : ISecurityModelFactory
    {
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;

        public SecurityModelFactory(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        public async Task<PermissionRolesModel> PreparePermissionRolesModel(PermissionRolesModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var roles = _userService.GetRoles();
            model.AvailableRoles = roles.Select(role => role.ToModel<RoleModel>()).ToList();

            foreach (var permission in await _permissionService.GetAllPermissions())
            {
                model.AvailablePermissions.Add(new PermissionModel
                {
                    Name = permission.Name,
                    SystemName = permission.SystemName,
                });

                foreach (var role in roles)
                {
                    if (!model.Allowed.ContainsKey(permission.SystemName))
                        model.Allowed[permission.SystemName] = new Dictionary<int, bool>();

                    model.Allowed[permission.SystemName][role.Id] =
                        permission.PermissionRoles.Any(map => map.RoleId == role.Id);
                }
            }

            return model;
        }
    }
}