using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class BarcodeModel : BaseEntityModel
    {
        [Display(Name = "Format")]
        public string Format { get; set; }

        [Display(Name = "Prefix")]
        public string Prefix { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Display(Name = "Length")]
        public int Length { get; set; }

        [Display(Name = "Sequence")]
        public int? Sequence { get; set; }
    }
}