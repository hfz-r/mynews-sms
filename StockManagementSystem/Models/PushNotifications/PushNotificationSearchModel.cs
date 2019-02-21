using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.PushNotifications
{
    public class PushNotificationSearchModel : BaseSearchModel
    {
        public PushNotificationSearchModel()
        {
            SelectedStoreIds = new List<int>();
            SelectedNotificationCategoryIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            AvailableNotificationCategory = new List<SelectListItem>();
        }

        [Display(Name = "Store")]
        public IList<int> SelectedStoreIds { get; set; }

        [Display(Name = "Category")]
        public IList<int> SelectedNotificationCategoryIds { get; set; }

        public IList<SelectListItem> AvailableNotificationCategory { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        [Display(Name = "Title")]
        public string SearchTitle { get; set; }

        [Display(Name = "Description")]
        public string SearchDesc { get; set; }

        [Display(Name = "Stock Take #")]
        public string SearchStockTakeNo { get; set; }

    }
}
