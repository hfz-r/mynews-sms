using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.PushNotification;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static StockManagementSystem.Controllers.PushNotificationController;

namespace StockManagementSystem.Models.PushNotification
{
    public class AddNotificationViewModel
    {
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Desc { get; set; }

        [Display(Name = "Repeat")]
        public Repeat IsShift { get; set; }

        [Display(Name = "Category")]
        public int NotificationCategoryId { get; set; }

        public IEnumerable<NotificationCategory> NotificationCategories { set; get; }

        [Display(Name = "Branch")]
        public int StoreId { get; set; }

        public IEnumerable<Store> Stores { set; get; }

    }
}
