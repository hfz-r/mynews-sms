using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.PushNotifications
{
    public class PushNotificationModel : BaseEntityModel
    {
        public PushNotificationModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            SelectedNotificationCategoryIds = new List<int>();
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
        [MaxLength(4)]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid stock take number")]
        public string StockTakeNo { get; set; }

        [Display(Name = "Store")]
        public string StoreName { get; set; }

        [Display(Name = "Stores")]
        [Required(ErrorMessage = "Store is required")]
        public IList<int> SelectedStoreIds { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Categories")]
        [Required(ErrorMessage = "Notification Category is required")]
        public IList<int> SelectedNotificationCategoryIds { get; set; }

        public IList<SelectListItem> AvailableNotificationCategories { get; set; }

        public ICollection<Core.Domain.PushNotifications.PushNotification> PushNotifications { get; set; }
    }
}
