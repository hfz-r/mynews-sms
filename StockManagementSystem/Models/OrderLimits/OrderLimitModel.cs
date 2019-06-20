using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Master;
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
            SelectedStoreIds = new int();
            AvailableStores = new List<SelectListItem>();
        }

        public string Name { get; set; }

        //Remove Percentage criteria; Not required - 05032019
        //[Display(Name = "Percentage")]
        //[Required(ErrorMessage = "Percentage is required")]
        //[DataType("Integer")]
        //public int Percentage { get; set; }

        [Display(Name = "Delivery Per Week")]
        [Required(ErrorMessage = "Delivery Per Week is required")]
        [DataType("Integer")]
        public int DeliveryPerWeek { get; set; }

        [Display(Name = "Safety")]
        [Required(ErrorMessage = "Safety is required")]
        [DataType("Integer")]
        public int Safety { get; set; }

        [Display(Name = "Inventory Cycle")]
        [Required(ErrorMessage = "Inventory Cycle is required")]
        [DataType("Integer")]
        public int InventoryCycle { get; set; }

        [Display(Name = "Order Ratio")]
        [Required(ErrorMessage = "Order Ratio is required")]
        public float OrderRatio { get; set; }

        [Display(Name = "Min Day(s)")]
        [Required(ErrorMessage = "Min Day(s) is required")]
        public int MinDays { get; set; }

        [Display(Name = "Max Day(s)")]
        [Required(ErrorMessage = "Max Day(s) is required")]
        public int MaxDays { get; set; }

        [Display(Name = "Face Qty")]
        [Required(ErrorMessage = "Face Qty is required")]
        public int FaceQty { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last activity")]
        public DateTime LastActivityDate { get; set; }

        [Display(Name = "Store")]
        public string StoreName { get; set; }

        [Display(Name = "Stores")]
        [Required(ErrorMessage = "Store is required")]
        public int SelectedStoreIds { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public ICollection<OrderBranchMaster> OrderLimits { get; set; }
    }
}
