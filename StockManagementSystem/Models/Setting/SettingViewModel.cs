using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models.Setting
{
    public class SettingViewModel
    {
        public int? OrderLimitId { get; set; }

        [Required(ErrorMessage = "Percentage is required.")]
        [Display(Name = "Percentage")]
        public double Percentage { get; set; }

        [Display(Name = "BranchNo")]
        public int P_BranchNo { get; set; }

        public ICollection<OrderLimit> OrderLimit { get; set; }

        public List<Store> Store { get; set; }

        //Location Setting
        public int? ShelfLocationFormatId { get; set; }

        public int? ShelfLocationId { get; set; }

        public string Prefix { get; set; }

        public string Names { get; set; }

        public string Location { get; set; }

        [Display(Name = "Item(s)")]
        public int ItemId { get; set; }

        public ICollection<ShelfLocationFormat> ShelfLocationFormats { get; set; }

        public ICollection<ShelfLocation> ShelfLocations { get; set; }

        public List<Item> Item { get; set; }
    }
}
