using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Logging
{
    public class ActivityLogSearchModel : BaseSearchModel
    {
        public ActivityLogSearchModel()
        {
            ActivityLogType = new List<SelectListItem>();
        }

        [Display(Name = "Created from")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [Display(Name = "Created to")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [Display(Name = "Activity log type")]
        public int ActivityLogTypeId { get; set; }

        [Display(Name = "Activity log type")]
        public IList<SelectListItem> ActivityLogType { get; set; }

        [Display(Name = "IP address")]
        public string IpAddress { get; set; }
    }
}