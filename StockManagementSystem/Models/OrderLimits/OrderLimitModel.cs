using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.OrderLimits
{
    public class OrderLimitModel : BaseEntityModel
    {
        public OrderLimitModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        public string Name { get; set; }

        [Display(Name = "Percentage")]
        [Required(ErrorMessage = "Percentage is required")]
        [DataType("Integer")]
        public int Percentage { get; set; }

        [Display(Name = "Days of Stock")]
        [Required(ErrorMessage = "Days of Stock is required")]
        [DataType("Integer")]
        public int DaysofStock { get; set; }

        [Display(Name = "Days of Sales")]
        [Required(ErrorMessage = "Days of Sales is required")]
        [DataType("Integer")]
        public int DaysofSales { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last activity")]
        public DateTime LastActivityDate { get; set; }

        [Display(Name = "Store")]
        public string StoreName { get; set; }

        [Display(Name = "Stores")]
        [Required(ErrorMessage = "Store is required")]
        public IList<int> SelectedStoreIds { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public ICollection<OrderLimit> OrderLimits { get; set; }
    }
}
