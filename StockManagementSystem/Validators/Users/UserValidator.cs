using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Data;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Services.Roles;
using StockManagementSystem.Web.Validators;

namespace StockManagementSystem.Validators.Users
{
    public class UserValidator : BaseValidator<UserModel>
    {
        public UserValidator(IRoleService roleService, IDbContext dbContext)
        {
            //ensure that valid email address is entered if Registered role is checked
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Wrong email.")
                //only for registered users
                .When(x => IsRegisteredRoleChecked(x, roleService));

            //name
            RuleFor(x => x.Name).NotEmpty().WithMessage("Full name is required.");

            SetDatabaseValidationRules<User>(dbContext);
        }

        private bool IsRegisteredRoleChecked(UserModel model, IRoleService roleService)
        {
            var allRoles = roleService.GetRolesAsync().GetAwaiter().GetResult();
            var newRoles = new List<Role>();
            foreach (var role in allRoles)
            {
                if (model.SelectedRoleIds.Contains(role.Id))
                    newRoles.Add(role);
            }

            var isInRegisteredRole = newRoles.FirstOrDefault(r => r.SystemName == IdentityDefaults.RegisteredRoleName) != null;
            return isInRegisteredRole;
        }
    }
}