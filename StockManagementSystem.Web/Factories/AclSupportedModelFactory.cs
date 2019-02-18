using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Factories
{
    public class AclSupportedModelFactory : IAclSupportedModelFactory
    {
        private readonly IAclService _aclService;
        private readonly IUserService _userService;

        public AclSupportedModelFactory(
            IAclService aclService,
            IUserService userService)
        {
            _aclService = aclService;
            _userService = userService;
        }

        /// <summary>
        /// Prepare selected and all available user roles for the passed model
        /// </summary>
        public void PrepareModelRoles<TModel>(TModel model) where TModel : IAclSupportedModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var availableRoles = _userService.GetRoles(showHidden: true);
            model.AvailableRoles = availableRoles.Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString(),
                Selected = model.SelectedRoleIds.Contains(role.Id)
            }).ToList();
        }

        /// <summary>
        /// Prepare selected and all available user roles for the passed model by ACL mappings
        /// </summary>
        public async Task PrepareModelRoles<TModel, TEntity>(TModel model, TEntity entity, bool ignorePermissionMappings)
            where TModel : IAclSupportedModel 
            where TEntity : BaseEntity, IAclSupported
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!ignorePermissionMappings && entity != null)
                model.SelectedRoleIds = await _aclService.GetRoleIdsWithAccess(entity);

            PrepareModelRoles(model);
        }
    }
}