using System;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Media;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Factories;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Media;
using StockManagementSystem.Services.Stores;

namespace StockManagementSystem.Factories
{
    /// <summary>
    /// Represents the user model factory implementation
    /// </summary>
    public class UserModelFactory : IUserModelFactory
    {
        private readonly UserSettings _userSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IBaseModelFactory _baseModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        private readonly IAppliedStoreSupportedModelFactory _appliedStoreSupportedModelFactory;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;

        public UserModelFactory(
            UserSettings userSettings,
            MediaSettings mediaSettings,
            IUserService userService,
            IUserActivityService userActivityService,
            IBaseModelFactory baseModelFactory,
            IGenericAttributeService genericAttributeService,
            IStoreService storeService,
            IPictureService pictureService,
            IAppliedStoreSupportedModelFactory appliedStoreSupportedModelFactory,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IDateTimeHelper dateTimeHelper)
        {
            _userSettings = userSettings;
            _mediaSettings = mediaSettings;
            _userService = userService;
            _userActivityService = userActivityService;
            _baseModelFactory = baseModelFactory;
            _genericAttributeService = genericAttributeService;
            _storeService = storeService;
            _pictureService = pictureService;
            _appliedStoreSupportedModelFactory = appliedStoreSupportedModelFactory;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _dateTimeHelper = dateTimeHelper;
        }

        public Task<UserSearchModel> PrepareUserSearchModel(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.UsernamesEnabled = _userSettings.UsernamesEnabled;
            searchModel.AvatarEnabled = _userSettings.AllowUsersToUploadAvatars;
            searchModel.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            searchModel.PhoneEnabled = _userSettings.PhoneEnabled;

            // this is required! please check related code regarding this before commented.
            var registeredRole = _userService.GetRoleBySystemName(UserDefaults.RegisteredRoleName);
            if (registeredRole != null)
                searchModel.SelectedRoleIds.Add(registeredRole.Id);

            registeredRole = _userService.GetRoleBySystemName(UserDefaults.AdministratorsRoleName);
            if (registeredRole != null)
                searchModel.SelectedRoleIds.Add(registeredRole.Id);

            //registeredRole = _userService.GetRoleBySystemName(UserDefaults.SysAdminRoleName);
            //if (registeredRole != null)
            //    searchModel.SelectedRoleIds.Add(registeredRole.Id);

            //registeredRole = _userService.GetRoleBySystemName(UserDefaults.GuestsRoleName);
            //if (registeredRole != null)
            //    searchModel.SelectedRoleIds.Add(registeredRole.Id);

            registeredRole = _userService.GetRoleBySystemName(UserDefaults.CashierRoleName);
            if (registeredRole != null)
                searchModel.SelectedRoleIds.Add(registeredRole.Id);

            //prepare available user roles
            _aclSupportedModelFactory.PrepareModelRoles(searchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        public async Task<UserListModel> PrepareUserListModel(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter customers
            int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
            int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);

            var users = await _userService.GetUsersAsync(
                roleIds: searchModel.SelectedRoleIds.ToArray(),
                email: searchModel.SearchEmail,
                username: searchModel.SearchUsername,
                firstName: searchModel.SearchFirstName,
                lastName: searchModel.SearchLastName,
                dayOfBirth: dayOfBirth,
                monthOfBirth: monthOfBirth,
                phone: searchModel.SearchPhone,
                ipAddress: searchModel.SearchIpAddress,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new UserListModel
            {
                Data = users.Select(user => 
                {
                    var userModel = user.ToModel<UserModel>();

                    userModel.Email = user.Email;
                    userModel.FullName = _userService.GetUserFullNameAsync(user).GetAwaiter().GetResult();
                    userModel.Phone = _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.PhoneAttribute).GetAwaiter().GetResult();
                    userModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
                    userModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);
                    userModel.UserRolesName = String.Join(", ", user.UserRoles.Select(role => role.Role.Name));

                    if (_userSettings.AllowUsersToUploadAvatars)
                    {
                        var avatarPictureId = _genericAttributeService.GetAttributeAsync<int>(user, UserDefaults.AvatarPictureIdAttribute).GetAwaiter().GetResult();
                        userModel.AvatarUrl = _pictureService.GetPictureUrl(avatarPictureId,  _mediaSettings.AvatarPictureSize, _userSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                    }

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

        /// <summary>
        /// Prepare user model
        /// </summary>
        /// <param name="model">User model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns></returns>
        public async Task<UserModel> PrepareUserModel(UserModel model, User user, bool excludeProperties = false)
        {
            if (user != null)
            {
                model = model ?? new UserModel();

                model.Id = user.Id;
                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.Email = user.Email;
                    model.Username = user.Username;
                    model.Barcode = user.Barcode;
                    model.Active = user.Active;
                    model.FirstName = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.FirstNameAttribute);
                    model.LastName = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.LastNameAttribute);
                    model.Gender = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.GenderAttribute);
                    model.DateOfBirth = await _genericAttributeService.GetAttributeAsync<DateTime?>(user, UserDefaults.DateOfBirthAttribute);
                    model.Phone = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.PhoneAttribute);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
                    model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = user.LastIpAddress;
                    model.LastVisitedPage = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.LastVisitedPageAttribute);
                    model.SelectedRoleIds = user.UserRoles.Select(map => map.RoleId).ToList();
                }
                //prepare nested search models
                PrepareUserActivityLogSearchModel(model.UserActivityLogSearchModel, user);
            }
            else
            {
                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    //precheck Registered Role 
                    var registeredRole = _userService.GetRoleBySystemName(UserDefaults.RegisteredRoleName);
                    if (registeredRole != null)
                        model.SelectedRoleIds.Add(registeredRole.Id);
                }
            }

            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;

            //set default values for the new model
            if (user == null)
                model.Active = true;

            //prepare model user roles
            _aclSupportedModelFactory.PrepareModelRoles(model);

            //prepare model user stores
            var availableStores = await _storeService.GetStores();
            _appliedStoreSupportedModelFactory.PrepareModelAppliedStores(model, user, availableStores, excludeProperties);

            //prepare available time zones
            await _baseModelFactory.PrepareTimeZones(model.AvailableTimeZones, false);

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
                    userActivityLogModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

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