using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Security;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Roles;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class SecurityController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly ISecurityModelFactory _securityModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IWorkContext _workContext;

        public SecurityController(
            IRoleService roleService,
            ISecurityModelFactory securityModelFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            IWorkContext workContext,
            ILogger<SecurityController> logger )
        {
            _roleService = roleService;
            _securityModelFactory = securityModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _workContext = workContext;

            Logger = logger;
        }

        public ILogger Logger { get; }

        public IActionResult AccessDenied(string pageUrl)
        {
            var currentUser = _workContext.CurrentUser;
            if (currentUser == null || !User.Identity.IsAuthenticated)
            {
                Logger.LogWarning($"Access denied to anonymous request on {pageUrl}");
                return View();
            }

            Logger.LogWarning($"Access denied to user #{currentUser.Email} '{currentUser.Email}' on {pageUrl}");

            return View();
        }

        public async Task<IActionResult> Permissions()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePermission))
                return AccessDeniedView();

            var model = await _securityModelFactory.PreparePermissionRolesModel(new PermissionRolesModel());

            return View(model);
        }

        [HttpPost, ActionName("Permissions")]
        public async Task<IActionResult> PermissionsSave(PermissionRolesModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePermission))
                return AccessDeniedView();

            var permissions = await _permissionService.GetAllPermissions();
            var roles = await _roleService.GetRolesAsync();

            foreach (var role in roles)
            {
                var formKey = "allow_" + role.Id;
                var permissionSystemNamesToRestrict = !StringValues.IsNullOrEmpty(model.Form[formKey])
                    ? model.Form[formKey].ToString().Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>();

                foreach (var p in permissions)
                {
                    var allow = permissionSystemNamesToRestrict.Contains(p.SystemName);
                    if (allow)
                    {
                        if (p.PermissionRoles.FirstOrDefault(x => x.RoleId == role.Id) != null)
                            continue;

                        p.PermissionRoles.Add(new PermissionRoles{Role = role});
                        _permissionService.UpdatePermission(p);
                    }
                    else
                    {
                        if (p.PermissionRoles.FirstOrDefault(x => x.RoleId == role.Id) == null)
                            continue;

                        p.PermissionRoles.Remove(p.PermissionRoles.FirstOrDefault(map => map.RoleId == role.Id));
                        _permissionService.UpdatePermission(p);
                    }
                }
            }

            _notificationService.SuccessNotification("The permission has been updated successfully");

            return RedirectToAction("Permissions");
        }
    }
}