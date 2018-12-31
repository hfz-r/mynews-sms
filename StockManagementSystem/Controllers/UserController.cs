using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Roles;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc.Filters;

namespace StockManagementSystem.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserModelFactory _userModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;

        public UserController(
            IUserService userService,
            IRoleService roleService,
            IUserModelFactory userModelFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _userModelFactory = userModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;

            Logger = logger;
        }

        public ILogger Logger { get; }

        #region Utilities

        protected virtual string ValidateRoles(IList<Role> roles)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            //ensure user not added to 'Registered' role
            //ensure user is in required role 'Registered'
            var isInRegisteredRole =
                roles.FirstOrDefault(r => r.SystemName == IdentityDefaults.RegisteredRoleName) != null;
            if (!isInRegisteredRole)
                return "Add user to 'Registered' role";

            //no errors
            return string.Empty;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var model = await _userModelFactory.PrepareUserSearchModel(new UserSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserList(UserSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            var model = await _userModelFactory.PrepareUserListModel(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return RedirectToAction("Index");

            var model = await _userModelFactory.PrepareUserModel(null, user);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> Edit(UserModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = await _userService.GetUserByIdAsync(model.Id);
            if (user == null)
                return RedirectToAction("Index");

            //validate user roles
            var allRoles = await _roleService.GetRolesAsync();
            var newRoles = new List<Role>();
            foreach (var role in allRoles)
            {
                if (model.SelectedRoleIds.Contains(role.Id))
                    newRoles.Add(role);
            }

            var rolesError = ValidateRoles(newRoles);
            if (!string.IsNullOrEmpty(rolesError))
            {
                ModelState.AddModelError(string.Empty, rolesError);
                _notificationService.ErrorNotification(rolesError);
            }

            if (newRoles.Any() && newRoles.FirstOrDefault(c => c.SystemName == IdentityDefaults.RegisteredRoleName) != null && !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Valid Email is required for user to be in 'Registered' role");
                _notificationService.ErrorNotification("Valid Email is required for user to be in 'Registered' role");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Name = model.Name;
                    user.AdminComment = model.AdminComment;

                    //email
                    if (!string.IsNullOrWhiteSpace(model.Email))
                        await _userService.SetEmail(user, model.Email);
                    else
                        user.Email = model.Email;
                 
                    //roles
                    var rolesStr = new List<string>();
                    foreach (var role in allRoles)
                    {
                        if (model.SelectedRoleIds.Contains(role.Id))
                        {
                            //new role
                            if (user.UserRoles.Count(mapping => mapping.RoleId == role.Id) == 0)
                                rolesStr.Add(role.Name);
                        }
                        else
                        {
                            //remove role
                            if (user.UserRoles.Count(mapping => mapping.RoleId == role.Id) > 0)
                                await _userService.RemoveUserRole(user, role.Name);
                        }
                    }
                    await _userService.AddUserRoles(user, rolesStr.ToArray());
                    await _userService.UpdateUserAsync(user);

                    _notificationService.SuccessNotification("User has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("Index");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = user.Id });
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }
            }

            model = await _userModelFactory.PrepareUserModel(model, user);

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return RedirectToAction("Index");

            try
            {
                //remove
                await _userService.DeleteUserAsync(user);

                _notificationService.SuccessNotification("User has been deleted successfully.");

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return RedirectToAction("Edit", new { id = user.Id });
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public async Task<IActionResult> ChangePassword(UserModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = await _userService.GetUserByIdAsync(model.Id);
            if (user == null)
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
                return RedirectToAction("Edit", new { id = user.Id });

            try
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                    throw new Exception("Password is required to use this operation.");

                var changePassResult = await _userService.ChangePassword(user, model.Password);
                if (changePassResult.Succeeded)
                    _notificationService.SuccessNotification("The password has been changed successfully.");
                else
                {
                    foreach (var error in changePassResult.Errors)
                        _notificationService.ErrorNotification($"{error.Code} - {error.Description}");
                }
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }
    }
}