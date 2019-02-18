using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Factories;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Web.Mvc;

namespace StockManagementSystem.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleModelFactory _roleModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;

        public RoleController(
            IRoleModelFactory roleModelFactory, 
            IPermissionService permissionService, 
            IUserService userService, 
            IUserActivityService userActivityService)
        {
            _roleModelFactory = roleModelFactory;
            _permissionService = permissionService;
            _userService = userService;
            _userActivityService = userActivityService;
        }

        public async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var model = await _roleModelFactory.PrepareRoleSearchModel(new RoleSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> List(RoleSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            var model = await _roleModelFactory.PrepareRoleListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAdd(RoleModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});

            var role = new Role();
            role = model.ToEntity(role);

            await _userService.InsertRoleAsync(role);

            //activity log
            await _userActivityService.InsertActivityAsync("AddNewRole", $"Added a new role ('{role.Name}')", role);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});

            var role = await _userService.GetRoleByIdAsync(model.Id);

            if (role.IsSystemRole && !role.Active)
                throw new DefaultException("System roles can't be disabled.");
            if (role.IsSystemRole && !role.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                throw new DefaultException("The system name of system roles can't be edited.");

            role = model.ToEntity(role);
            await _userService.UpdateRoleAsync(role);
            
            //activity log
            await _userActivityService.InsertActivityAsync("EditRole", $"Edited a user role ('{role.Name}')", role);

            return new NullJsonResult();
        }

        public async Task<IActionResult> RoleDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var role = await _userService.GetRoleByIdAsync(id) ?? throw new ArgumentException("No role found with the specified id", nameof(id));
            await _userService.DeleteRoleAsync(role);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteRole", $"Deleted a user role ('{role.Name}')", role);

            return new NullJsonResult();
        }
    }
}