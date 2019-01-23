using System;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Roles;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Factories;
using StockManagementSystem.Web.Kendoui.Extensions;

namespace StockManagementSystem.Factories
{
    /// <summary>
    /// Represents the user model factory implementation
    /// </summary>
    public class UserModelFactory : IUserModelFactory
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserActivityService _userActivityService;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;

        public UserModelFactory(
            IUserService userService,
            IRoleService roleService,
            IUserActivityService userActivityService,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IDateTimeHelper dateTimeHelper)
        {
            _userService = userService;
            _roleService = roleService;
            _userActivityService = userActivityService;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<UserSearchModel> PrepareUserSearchModel(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //Commented on 23 Jan 2019, myNEWS have own roles.
            ////prepare role selectlist = default = registered 
            //var defaultRole = await _roleService.GetRoleBySystemNameAsync(IdentityDefaults.RegisteredRoleName);
            //if (defaultRole != null)
            //    searchModel.SelectedRoleIds.Add(defaultRole.Id);

            await _aclSupportedModelFactory.PrepareModelRoles(searchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public async Task<UserListModel> PrepareUserListModel(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var users = await _userService.GetUsersAsync(
                roleIds: searchModel.SelectedRoleIds.ToArray(),
                email: searchModel.SearchEmail,
                username: searchModel.SearchUsername,
                name: searchModel.SearchName,
                ipAddress: searchModel.SearchIpAddress,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new UserListModel
            {
                Data = users.Select(user =>
                {
                    var userModel = user.ToModel<UserModel>();

                    userModel.Email = user.Email;
                    userModel.Username = user.UserName;
                    userModel.Name = user.Name;
                    userModel.UserRolesName = String.Join(", ", user.UserRoles.Select(role => role.Role.Name));
                    userModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
                    userModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);

                    return userModel;
                }),
                Total = users.TotalCount
            };

            // sort
            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            // filter
            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        public async Task<UserModel> PrepareUserModel(UserModel model, User user)
        {
            if (user != null)
            {
                model = model ?? new UserModel();

                model.Id = user.Id;
                model.Email = user.Email;
                model.Username = user.UserName;
                model.Name = user.Name;
                model.AdminComment = user.AdminComment;
                model.LastIpAddress = user.LastIpAddress;
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
                model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);
                model.SelectedRoleIds = user.UserRoles.Select(map => map.RoleId).ToList();

                PrepareUserActivityLogSearchModel(model.UserActivityLogSearchModel, user);
            }
            else
            {
                //precheck Registered Role 
                var registeredRole = await _roleService.GetRoleBySystemNameAsync(IdentityDefaults.RegisteredRoleName);
                if (registeredRole != null)
                    model.SelectedRoleIds.Add(registeredRole.Id);
            }
            //prepare model user roles
            await _aclSupportedModelFactory.PrepareModelRoles(model);

            return model;
        }

        public UserActivityLogSearchModel PrepareUserActivityLogSearchModel(UserActivityLogSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            searchModel.UserId = user.Id;
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public async Task<UserActivityLogListModel> PrepareUserActivityLogListModel(UserActivityLogSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var activityLog = _userActivityService.GetAllActivities(
                userId: user.Id, 
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new UserActivityLogListModel
            {
                Data = activityLog.Select(logItem =>
                {
                    var userActivityLogModel = logItem.ToModel<UserActivityLogModel>();
                    userActivityLogModel.ActivityLogTypeName = logItem.ActivityLogType.Name;
                    userActivityLogModel.CreatedOn =  _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return userActivityLogModel;
                }),
                Total = activityLog.TotalCount
            };

            // sort
            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            // filter
            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }
    }
}