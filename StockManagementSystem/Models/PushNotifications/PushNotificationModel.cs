﻿using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.PushNotifications
{
    public class PushNotificationModel : BaseEntityModel
    {
        public class PushNoti
        {
            public string STNo { get; set; }
        }

        public enum RepeatEnum
        {
            Never,
            Daily,
            Week,
            Monthly
        }

        public enum RepeatDayEnum
        {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday
        }

        public PushNotificationModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            AvailableStockTakeList = new List<SelectListItem>();
            OutletList = new List<SelectListItem>();
            SelectedNotificationCategoryIds = new List<int>();
            SelectedStockTake = new List<int?>();
            AvailableNotificationCategories = new List<SelectListItem>();
        }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last activity")]
        public DateTime LastActivityDate { get; set; }

        [Display(Name = "ST#")]
        public string StockTakeNo { get; set; }

        [Display(Name = "ST#")]
        public List<StockTakeHeader> StockTakeList { get; set; }

        public class StoreList
        {
            public string StoreNo { get; set; }
            public string StoreName { get; set; }
        }

        public class StockTakeHeader
        {
            public string StockTakeNo { get; set; }
            public List<StoreList> Stores { get; set; }
        }

        [Display(Name = "Store")]
        public string StoreName { get; set; }

        [Display(Name = "Stores")]
        //[Required(ErrorMessage = "Store is required")]
        public IList<int> SelectedStoreIds { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableStockTakeList { get; set; }

        public IList<SelectListItem> OutletList { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Notification Category is required")]
        public IList<int> SelectedNotificationCategoryIds { get; set; }

        [Display(Name = "Stock Take #")]
        public IList<int?> SelectedStockTake { get; set; }

        [Display(Name = "Remind me on a day")]
        public bool RemindMe { get; set; }

        [Display(Name = "Repeats")]
        public IList<string> Repeat { get; set; }

        public IList<string> RepeatDay { get; set; }

        [Display(Name = "Every")]
        public int? RepeatEvery { get; set; }

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        public string RepeatEveryLabel { get; set; }

        public IList<SelectListItem> AvailableNotificationCategories { get; set; }

        public ICollection<Core.Domain.PushNotifications.PushNotification> PushNotifications { get; set; }
    }
}
