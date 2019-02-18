using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Setting
{
    public class ShelfModel : BaseEntityModel
    {
        [Display(Name = "Format")]
        public string Format { get; set; }

        [Display(Name = "Prefix")]
        public string Prefix { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
