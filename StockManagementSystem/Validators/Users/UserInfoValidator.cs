using System;
using FluentValidation;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Models.Account;
using StockManagementSystem.Web.Validators;

namespace StockManagementSystem.Validators.Users
{
    public class UserInfoValidator : BaseValidator<UserInfoModel>
    {
        public UserInfoValidator(UserSettings userSettings)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Wrong email");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");

            if (userSettings.UsernamesEnabled && userSettings.AllowUsersToChangeUsernames)
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
                RuleFor(x => x.Username).IsUsername(userSettings).WithMessage("Username is not valid");
            }

            //form fields
            if (userSettings.DateOfBirthEnabled && userSettings.DateOfBirthRequired)
            {
                //entered?
                RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
                    {
                        var dateOfBirth = x.ParseDateOfBirth();
                        if (!dateOfBirth.HasValue)
                            return false;   

                        return true;
                    })
                    .WithMessage("Date of birth is required.");

                //minimum age
                RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
                {
                    var dateOfBirth = x.ParseDateOfBirth();
                    if (dateOfBirth.HasValue && userSettings.DateOfBirthMinimumAge.HasValue &&
                        CommonHelper.GetDifferenceInYears(dateOfBirth.Value, DateTime.Today) < userSettings.DateOfBirthMinimumAge.Value)
                        return false;

                    return true;
                }).WithMessage($"You have to be {userSettings.DateOfBirthMinimumAge}");
            }

            if (userSettings.PhoneRequired && userSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required.");
            }
        }
    }
}