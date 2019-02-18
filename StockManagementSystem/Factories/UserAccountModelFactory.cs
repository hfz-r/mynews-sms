using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Helpers;

namespace StockManagementSystem.Factories
{
    /// <summary>
    /// Represents the user account model factory
    /// </summary>
    public class UserAccountModelFactory : IUserAccountModelFactory
    {
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        public UserAccountModelFactory(
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            IDateTimeHelper dateTimeHelper, 
            IGenericAttributeService genericAttributeService, 
            IWorkContext workContext)
        {
            _userSettings = userSettings;
            _dateTimeSettings = dateTimeSettings;
            _dateTimeHelper = dateTimeHelper;
            _genericAttributeService = genericAttributeService;
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
    }
}