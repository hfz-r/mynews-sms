using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Media;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Media;

namespace StockManagementSystem.Factories
{
    /// <summary>
    /// Represents the user account model factory
    /// </summary>
    public class UserAccountModelFactory : IUserAccountModelFactory
    {
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;

        public UserAccountModelFactory(
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            MediaSettings mediaSettings,
            IDateTimeHelper dateTimeHelper, 
            IGenericAttributeService genericAttributeService,
            IPictureService pictureService,
            IWorkContext workContext)
        {
            _userSettings = userSettings;
            _dateTimeSettings = dateTimeSettings;
            _mediaSettings = mediaSettings;
            _dateTimeHelper = dateTimeHelper;
            _genericAttributeService = genericAttributeService;
            _pictureService = pictureService;
            _workContext = workContext;
        }

        public Task<LoginViewModel> PrepareLoginModel()
        {
            var model = new LoginViewModel { UsernamesEnabled = _userSettings.UsernamesEnabled };
            return Task.FromResult(model);
        }

        public Task<RegisterViewModel> PrepareRegisterModel(RegisterViewModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem
                {
                    Text = tzi.DisplayName,
                    Value = tzi.Id,
                    Selected = (excludeProperties
                        ? tzi.Id == model.TimeZoneId
                        : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id)
                });

            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _userSettings.DateOfBirthRequired;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;

            return Task.FromResult(model);
        }

        public Task<ForgotPasswordViewModel> PrepareForgotPasswordModel()
        {
            var model = new ForgotPasswordViewModel();
            return Task.FromResult(model);
        }

        public Task<ForgotPasswordConfirmViewModel> PrepareForgotPasswordConfirmModel()
        {
            var model = new ForgotPasswordConfirmViewModel();
            return Task.FromResult(model);
        }

        public Task<ChangePasswordModel> PrepareChangePasswordModel()
        {
            var model = new ChangePasswordModel();
            return Task.FromResult(model);
        }

        public Task<UserNavigationModel> PrepareUserNavigationModel(int selectedTabId = 0)
        {
            var model = new UserNavigationModel();

            model.UserNavigationItem.Add(new UserNavigationItemModel
            {
                RouteName = "UserInfo",
                Title = "User info",
                Tab = UserNavigationEnum.Info,
                ItemClass = "user-info"
            });

            model.UserNavigationItem.Add(new UserNavigationItemModel
            {
                RouteName = "UserChangePassword",
                Title = "Change password",
                Tab = UserNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_userSettings.AllowUsersToUploadAvatars)
            {
                model.UserNavigationItem.Add(new UserNavigationItemModel
                {
                    RouteName = "UserAvatar",
                    Title = "Avatar",
                    Tab = UserNavigationEnum.Avatar,
                    ItemClass = "user-avatar"
                });
            }

            model.SelectedTab = (UserNavigationEnum)selectedTabId;

            return Task.FromResult(model);
        }

        public async Task<UserInfoModel> PrepareUserInfoModel(UserInfoModel model, User user, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem
                {
                    Text = tzi.DisplayName,
                    Value = tzi.Id,
                    Selected = (excludeProperties
                        ? tzi.Id == model.TimeZoneId
                        : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id)
                });

            if (!excludeProperties)
            {
                model.FirstName = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.FirstNameAttribute);
                model.LastName = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.LastNameAttribute);
                model.Gender = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.GenderAttribute);
                var dateOfBirth = await _genericAttributeService.GetAttributeAsync<DateTime?>(user, UserDefaults.DateOfBirthAttribute);
                if (dateOfBirth.HasValue)
                {
                    model.DateOfBirthDay = dateOfBirth.Value.Day;
                    model.DateOfBirthMonth = dateOfBirth.Value.Month;
                    model.DateOfBirthYear = dateOfBirth.Value.Year;
                }
                model.Phone = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.PhoneAttribute);
                model.Email = user.Email;
                model.Username = user.Username;
            }
            else
            {
                if (_userSettings.UsernamesEnabled && !_userSettings.AllowUsersToChangeUsernames)
                    model.Username = user.Username;
            }

            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _userSettings.DateOfBirthRequired;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;

            return model;
        }

        public async Task<UserAvatarModel> PrepareUserAvatarModel(UserAvatarModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvatarUrl = _pictureService.GetPictureUrl(await _genericAttributeService.GetAttributeAsync<int>(_workContext.CurrentUser,
                    UserDefaults.AvatarPictureIdAttribute), _mediaSettings.AvatarPictureSize, false);

            return model;
        }
    }
}