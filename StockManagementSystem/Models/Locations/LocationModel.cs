using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Locations
{
    public class LocationModel : BaseEntityModel
    {
        public int? ShelfLocationId { get; set; }

        public int? ItemId { get; set; }

        [Display(Name = "Prefix")]
        public string Prefix { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        //[Display(Name = "Branch(s)")]
        //public string P_Name { get; set; }

        [Display(Name = "Item(s)")]
        public string Desc { get; set; }

        public ICollection<ShelfLocationFormat> ShelfLocationFormats { get; set; }

        public ICollection<ShelfLocation> ShelfLocations { get; set; }

        public IList<SelectListItem> Items { get; set; }
    }
}
