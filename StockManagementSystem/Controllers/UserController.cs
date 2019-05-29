using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;

namespace StockManagementSystem.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IUserService _userService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserModelFactory _userModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IUserActivityService _userActivityService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        public UserController(
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            IUserService userService, 
            IUserRegistrationService userRegistrationService,
            IUserModelFactory userModelFactory, 
            IGenericAttributeService genericAttributeService, 
            IPermissionService permissionService, 
            INotificationService notificationService, 
            IUserActivityService userActivityService, 
            IStoreService storeService,
            IWorkContext workContext)
        {
            _userSettings = userSettings;
            _dateTimeSettings = dateTimeSettings;
            _userService = userService;
            _userRegistrationService = userRegistrationService;
            _userModelFactory = userModelFactory;
            _genericAttributeService = genericAttributeService;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _userActivityService = userActivityService;
            _storeService = storeService;
            _workContext = workContext;
        }

        #region Utilities

        protected virtual string ValidateRoles(IList<Role> roles)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            var isInGuestsRole = roles.FirstOrDefault(r => r.SystemName == UserDefaults.GuestsRoleName) != null;
            var isInRegisteredRole = roles.FirstOrDefault(r => r.SystemName == UserDefaults.RegisteredRoleName) != null;
            if (isInGuestsRole && isInRegisteredRole)
                return "User cannot be in both 'Guest' and 'Registered' roles";
            if (!isInGuestsRole && !isInRegisteredRole)
                return "Add user to 'Guest' or 'Registered' role";

            //no errors
            return string.Empty;
        }

        private async Task<bool> SecondAdminAccountExists(User user)
        {
            var users = await _userService.GetUsersAsync(roleIds: new[] {_userService.GetRoleBySystemName(UserDefaults.AdministratorsRoleName).Id});
            return users.Any(u => u.Active && u.Id != user.Id);
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
            if (user == null || user.Deleted)
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
            if (user == null || user.Deleted)
                return RedirectToAction("Index");

            //validate user roles
            var allRoles = _userService.GetRoles(true);
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

            if (newRoles.Any() && newRoles.FirstOrDefault(c => c.SystemName == UserDefaults.RegisteredRoleName) != null && 
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Valid Email is required for user to be in 'Registered' role");
                _notificationService.ErrorNotification("Valid Email is required for user to be in 'Registered' role");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //prevent deactivation of the last active administrator
                    if (!user.IsAdmin() || model.Active || await SecondAdminAccountExists(user))
                        user.Active = model.Active;
                    else
                        _notificationService.ErrorNotification(
                            "You can't deactivate the last administrator. At least one administrator account should exists.");

                    //email
                    if (!string.IsNullOrWhiteSpace(model.Email))
                        await _userRegistrationService.SetEmailAsync(user, model.Email);
                    else
                        user.Email = model.Email;

                    //username
                    if (_userSettings.UsernamesEnabled)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Username))
                            await _userRegistrationService.SetUsernameAsync(user, model.Username);
                        else
                            user.Username = model.Username;
                    }

                    //form fields
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                    if (_userSettings.GenderEnabled)
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.GenderAttribute, model.Gender);
                    await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.FirstNameAttribute, model.FirstName);
                    await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.LastNameAttribute, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.DateOfBirthAttribute, model.DateOfBirth);
                    if (_userSettings.PhoneEnabled)
                        await _genericAttributeService.SaveAttributeAsync(user, UserDefaults.PhoneAttribute, model.Phone);

                    //roles
                    foreach (var role in allRoles)
                    {
                        if (role.SystemName == UserDefaults.AdministratorsRoleName && !_workContext.CurrentUser.IsAdmin())
                            continue;

                        if (model.SelectedRoleIds.Contains(role.Id))
                        {
                            if (user.UserRoles.Count(mapping => mapping.RoleId == role.Id) == 0)
                                user.AddUserRole(new UserRole{Role =  role});
                        }
                        else
                        {
                            //prevent attempts to delete the administrator role from the user, if the user is the last active administrator
                            if (role.SystemName == UserDefaults.AdministratorsRoleName && !await SecondAdminAccountExists(user))
                            {
                                _notificationService.ErrorNotification("You can't remove the Administrator role. At least one administrator account should exists.");
                                continue;
                            }

                            //remove role
                            if (user.UserRoles.Count(mapping => mapping.RoleId == role.Id) > 0)
                                user.RemoveUserRole(user.UserRoles.FirstOrDefault(mapping => mapping.RoleId == role.Id));
                        }
                    }

                    //stores
                    var stores = await _storeService.GetStores();
                    foreach (var store in stores)
                    {
                        if (model.SelectedStoreIds != null && model.SelectedStoreIds.Contains(store.P_BranchNo))
                        {
                            //new store
                            if (user.UserStores.Count(mapping => mapping.StoreId == store.P_BranchNo) == 0)
                                user.UserStores.Add(new UserStore {Store = store});
                        }
                        else
                        {
                            //remove store
                            if (user.UserStores.Count(mapping => mapping.StoreId == store.P_BranchNo) > 0)
                                user.UserStores.Remove(user.UserStores.FirstOrDefault(mapping =>
                                    mapping.StoreId == store.P_BranchNo));
                        }
                    }

                    await _userService.UpdateUserAsync(user);

                    //activity log
                    await _userActivityService.InsertActivityAsync("EditUser", $"Edited a user (ID = {user.Id})", user);

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

            model = await _userModelFactory.PrepareUserModel(model, user, true);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteInline(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = await _userService.GetUserByIdAsync(id) ??
                       throw new ArgumentException("No user found with the specified id", nameof(id));

            //prevent attempts to delete the user, if it is the last active administrator
            if (user.IsAdmin() && !await SecondAdminAccountExists(user))
                throw new ArgumentException("You can't delete the last administrator. At least one administrator account should exists.");

            //ensure that the current user cannot delete "Administrators" if he's not an admin himself
            if (user.IsAdmin() && !_workContext.CurrentUser.IsAdmin())
                throw new ArgumentException("You're not allowed to delete administrators. Only administrators can do it.");

            await _userService.DeleteUserAsync(user);

            await _userActivityService.InsertActivityAsync("DeleteUser", $"Deleted a user (ID = {user.Id})", user);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return RedirectToAction("Index");

            try
            {
                //prevent attempts to delete the user, if it is the last active administrator
                if (user.IsAdmin() && !await SecondAdminAccountExists(user))
                {
                    _notificationService.ErrorNotification("You can't delete the last administrator. At least one administrator account should exists.");
                    return RedirectToAction("Edit", new { id = user.Id });
                }

                //ensure that the current user cannot delete "Administrators" if he's not an admin himself
                if (user.IsAdmin() && !_workContext.CurrentUser.IsAdmin())
                {
                    _notificationService.ErrorNotification("You're not allowed to delete administrators. Only administrators can do it.");
                    return RedirectToAction("Edit", new { id = user.Id });
                }

                await _userService.DeleteUserAsync(user);

                //activity log
                await _userActivityService.InsertActivityAsync("DeleteUser", $"Deleted a user (ID = {user.Id})", user);

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

            //ensure that the current user cannot delete "Administrators" if he's not an admin himself
            if (user.IsAdmin() && !_workContext.CurrentUser.IsAdmin())
            {
                _notificationService.ErrorNotification("You're not allowed to delete administrators. Only administrators can do it.");
                return RedirectToAction("Edit", new { id = user.Id });
            }

            if (!ModelState.IsValid)
                return RedirectToAction("Edit", new { id = user.Id });

            var changePassRequest = new ChangePasswordRequest(model.Email, false, _userSettings.DefaultPasswordFormat, model.Password);
            var changePassResult = await _userRegistrationService.ChangePasswordAsync(changePassRequest);
            if (changePassResult.Success)
                _notificationService.SuccessNotification("The password has been changed successfully.");
            else
                foreach (var error in changePassResult.Errors)
                    _notificationService.ErrorNotification(error);

            return RedirectToAction("Edit", new { id = user.Id });
        }

        #region Activity Log

        [HttpPost]
        public async Task<IActionResult> ListActivityLog(UserActivityLogSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            var user = await _userService.GetUserByIdAsync(searchModel.UserId) 
                ?? throw new ArgumentException("No user found with the specified id");

            var model = await _userModelFactory.PrepareUserActivityLogListModel(searchModel, user);

            return Json(model);
        }

        #endregion
    }
}