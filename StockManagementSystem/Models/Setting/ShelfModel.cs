using StockManagementSystem.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Setting
{
    public class ShelfModel : BaseEntityModel
    {
        [Display(Name = "Format")]
        public string Format { get; set; }

        [Display(Name = "Prefix")]
        [MaxLength(1)]
        [Required(ErrorMessage = "Prefix is required")]
        public string Prefix { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
