using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Logging;
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
        private readonly IUserActivityService _userActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(
            IUserService userService,
            IRoleService roleService,
            IUserModelFactory userModelFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            IUserActivityService userActivityService,
            IDateTimeHelper dateTimeHelper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _userModelFactory = userModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _userActivityService = userActivityService;
            _dateTimeHelper = dateTimeHelper;
            _httpContextAccessor = httpContextAccessor;

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
                await _userService.DeleteUserAsync(user);
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

        #region Chart

        public async Task<IActionResult> LoadUserTransActivity(string period)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return Content(String.Empty);

            var result = new List<object>();

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var timeZone = _dateTimeHelper.CurrentTimeZone;
            var searchRoleIds = new[] { (await _roleService.GetRoleBySystemNameAsync(IdentityDefaults.RegisteredRoleName)).Id };

            var features = _httpContextAccessor.HttpContext?.Features?.Get<IRequestCultureFeature>();
            var culture = features?.RequestCulture.Culture;

            switch (period)
            {
                case "year":
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var yearToSearch = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (int i = 0; i <= 12; i++)
                    {
                        result.Add(new
                        {
                            date = yearToSearch.Date.ToString("Y", culture),
                            value = (await _userService.GetUsersAsync(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(yearToSearch, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(yearToSearch.AddMonths(1), timeZone),
                                roleIds: searchRoleIds, pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        yearToSearch = yearToSearch.AddMonths(1);
                    }
                    break;

                case "month":
                    var monthAgoDt = nowDt.AddDays(-30);
                    var monthToSearch = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (int i = 0; i <= 30; i++)
                    {
                        result.Add(new
                        {
                            date = monthToSearch.Date.ToString("M", culture),
                            value = (await _userService.GetUsersAsync(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(monthToSearch, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(monthToSearch.AddDays(1), timeZone),
                                roleIds: searchRoleIds, pageIndex: 0,  pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        monthToSearch = monthToSearch.AddDays(1);
                    }
                    break;

                case "week":
                    var weekAgoDt = nowDt.AddDays(-7);
                    var weekToSearch = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        result.Add(new
                        {
                            date = weekToSearch.Date.ToString("d dddd", culture),
                            value = (await _userService.GetUsersAsync(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(weekToSearch, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(weekToSearch.AddDays(1), timeZone),
                                roleIds: searchRoleIds, pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        weekToSearch = weekToSearch.AddDays(1);
                    }
                    break;
            }

            return Json(result);
        }

        public async Task<IActionResult> GetTransActivityDataPieChart(string period)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return Content(String.Empty);

            var result = new List<object>();

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var timeZone = _dateTimeHelper.CurrentTimeZone;
            var features = _httpContextAccessor.HttpContext?.Features?.Get<IRequestCultureFeature>();
            var culture = features?.RequestCulture.Culture;

            switch (period)
            {
                case "year":
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var yearToSearch = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (int i = 0; i <= 12; i++)
                    {
                        var activities = _userActivityService.GetAllActivities(
                                createdOnFrom: _dateTimeHelper.ConvertToUtcTime(yearToSearch, timeZone),
                                createdOnTo: _dateTimeHelper.ConvertToUtcTime(yearToSearch.AddMonths(1), timeZone))
                            .GroupBy(activity => activity.EntityName)
                            .Select(e => new
                            {
                                entity = e.Key,
                                total = e.Count().ToString()
                            });

                        foreach (var activity in activities)
                        {
                            result.Add(new
                            {
                                label = activity.entity,
                                value = activity.total
                            });
                        }

                        yearToSearch = yearToSearch.AddMonths(1);
                    }
                    break;

                case "month":
                    var monthAgoDt = nowDt.AddDays(-30);
                    var monthToSearch = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (int i = 0; i <= 30; i++)
                    {
                        var activities = _userActivityService.GetAllActivities(
                                createdOnFrom: _dateTimeHelper.ConvertToUtcTime(monthToSearch, timeZone),
                                createdOnTo: _dateTimeHelper.ConvertToUtcTime(monthToSearch.AddDays(1), timeZone))
                            .GroupBy(activity => activity.EntityName)
                            .Select(e => new
                            {
                                entity = e.Key,
                                total = e.Count().ToString()
                            });

                        foreach (var activity in activities)
                        {
                            result.Add(new
                            {
                                label = activity.entity,
                                value = activity.total
                            });
                        }

                        monthToSearch = monthToSearch.AddDays(1);
                    }
                    break;

                case "week":
                    var weekAgoDt = nowDt.AddDays(-7);
                    var weekToSearch = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        var activities = _userActivityService.GetAllActivities(
                                createdOnFrom: _dateTimeHelper.ConvertToUtcTime(weekToSearch, timeZone),
                                createdOnTo: _dateTimeHelper.ConvertToUtcTime(weekToSearch.AddDays(1), timeZone))
                            .GroupBy(activity => activity.EntityName)
                            .Select(e => new
                            {
                                entity = e.Key,
                                total = e.Count().ToString()
                            });

                        foreach (var activity in activities)
                        {
                            result.Add(new
                            {
                                label = activity.entity,
                                value = activity.total
                            });
                        }

                        weekToSearch = weekToSearch.AddDays(1);
                    }
                    break;
            }

            return Json(result);
        }

        #endregion
    }
}