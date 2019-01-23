using FluentValidation;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Data;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Web.Validators;

namespace StockManagementSystem.Validators.Roles
{
    public class RoleValidator : BaseValidator<RoleModel>
    {
        public RoleValidator(IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please provide a name.");

            SetDatabaseValidationRules<Role>(dbContext);
        }
    }
}