using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using StockManagementSystem.Validators.Roles;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Roles
{
    [Validator(typeof(RoleValidator))]
    public class RoleModel : BaseEntityModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Is system role")]
        public bool IsSystemRole { get; set; }

        [Display(Name = "System name")]
        public string SystemName { get; set; }
    }
}