using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Logging
{
    public class LogSearchModel : BaseSearchModel
    {
        public LogSearchModel()
        {
            AvailableLogLevels = new List<SelectListItem>();
        }

        [Display(Name = "Created from")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [Display(Name = "Created to")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Log level")]
        public int LogLevelId { get; set; }

        public IList<SelectListItem> AvailableLogLevels { get; set; }
    }
}