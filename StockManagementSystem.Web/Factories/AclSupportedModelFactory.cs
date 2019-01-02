using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Services.Roles;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Factories
{
    public class AclSupportedModelFactory : IAclSupportedModelFactory
    {
        private readonly IAclService _aclService;
        private readonly IRoleService _roleService;

        public AclSupportedModelFactory(
            IAclService aclService,
            IRoleService roleService)
        {
            _aclService = aclService;
            _roleService = roleService;
        }

        public async Task PrepareModelRoles<TModel>(TModel model) where TModel : IAclSupportedModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var roles = await _roleService.GetRolesAsync();
            model.AvailableRoles = roles.Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString(),
                Selected = model.SelectedRoleIds.Contains(role.Id)
            }).ToList();
        }

        public async Task PrepareModelRoles<TModel, TEntity>(TModel model, TEntity entity,
            bool ignorePermissionMappings)
            where TModel : IAclSupportedModel where TEntity : BaseEntity, IAclSupported
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ignorePermissionMappings && entity != null)
                model.SelectedRoleIds = await _aclService.GetRoleIdsWithAccess(entity);

            await PrepareModelRoles(model);
        }
    }
}