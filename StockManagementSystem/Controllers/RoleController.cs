﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Factories;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Roles;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Web.Mvc;

namespace StockManagementSystem.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleModelFactory _roleModelFactory;
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IUserActivityService _userActivityService;

        public RoleController(
            IRoleModelFactory roleModelFactory,
            IRoleService roleService,
            IPermissionService permissionService,
            IUserActivityService userActivityService)
        {
            _roleModelFactory = roleModelFactory;
            _roleService = roleService;
            _permissionService = permissionService;
            _userActivityService = userActivityService;
        }

        public async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRoles))
                return AccessDeniedView();

            var model = await _roleModelFactory.PrepareRoleSearchModel(new RoleSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> List(RoleSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRoles))
                return AccessDeniedKendoGridJson();

            var model = await _roleModelFactory.PrepareRoleListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAdd(RoleModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRoles))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});

            var role = new Role {SystemName = Regex.Replace(model.Name, @"\s", "")};
            role = model.ToEntity(role);

            await _roleService.InsertRoleAsync(role);
            await _userActivityService.InsertActivityAsync("AddNewRole", $"Added a new role ('{role.Name}')", role);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRoles))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});

            var role = await _roleService.GetRoleByIdAsync(model.Id);

            var sysName = Regex.Replace(model.Name, @"\s", "");
            if (role.SystemName != sysName)
                role.SystemName = sysName;

            role = model.ToEntity(role);

            await _roleService.UpdateRoleAsync(role);
            await _userActivityService.InsertActivityAsync("EditRole", $"Edited a user role ('{role.Name}')", role);

            return new NullJsonResult();
        }

        public async Task<IActionResult> RoleDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRoles))
                return AccessDeniedView();

            var role = await _roleService.GetRoleByIdAsync(id) ?? throw new ArgumentException("No role found with the specified id", nameof(id));
            await _roleService.DeleteRoleAsync(role);
            await _userActivityService.InsertActivityAsync("DeleteRole", $"Deleted a user role ('{role.Name}')", role);

            return new NullJsonResult();
        }
    }
}