using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Data;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Validators;

namespace StockManagementSystem.Validators.Users
{
    public class UserValidator : BaseValidator<UserModel>
    {
        public UserValidator(UserSettings userSettings, IUserService userService, IDbContext dbContext)
        {
            //ensure that valid email address is entered if Registered role is checked
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Wrong email.")
                //only for registered users
                .When(x => IsRegisteredRoleChecked(x, userService));

            //form fields
            if (userSettings.PhoneRequired && userSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone)
                    .NotEmpty()
                    .WithMessage("Phone is required")
                    //only for registered users
                    .When(x => IsRegisteredRoleChecked(x, userService));
            }

            SetDatabaseValidationRules<User>(dbContext);
        }

        private bool IsRegisteredRoleChecked(UserModel model, IUserService userService)
        {
            var allRoles = userService.GetRoles(true);
            var newRoles = new List<Role>();
            foreach (var role in allRoles)
            {
                if (model.SelectedRoleIds.Contains(role.Id))
                    newRoles.Add(role);
            }

            var isInRegisteredRole = newRoles.FirstOrDefault(r => r.SystemName == UserDefaults.RegisteredRoleName) != null;
            return isInRegisteredRole;
        }
    }
}