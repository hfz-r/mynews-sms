using FluentValidation.Attributes;
using StockManagementSystem.Validators.Roles;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Roles
{
    [Validator(typeof(RoleValidator))]
    public class RoleModel : BaseEntityModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}