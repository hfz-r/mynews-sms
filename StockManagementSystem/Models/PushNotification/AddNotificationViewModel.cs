using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.PushNotification;
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

        [Display(Name = "Shift")]
        public Repeat IsShift { get; set; }

        [Display(Name = "Category")]
        public int NotificationCategoryId { get; set; }

        public IEnumerable<NotificationCategory> NotificationCategories { set; get; }

    }
}
