using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class DateTimeSettingsModel : BaseModel, ISettingsModel
    {
        public DateTimeSettingsModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
        }

        public int ActiveTenantScopeConfiguration { get; set; }

        [Display(Name = "Allow users to select time zone")]
        public bool AllowUsersToSetTimeZone { get; set; }

        [Display(Name = "Default time zone")]
        public string DefaultTimeZoneId { get; set; }

        [Display(Name = "Default time zone")]
        public IList<SelectListItem> AvailableTimeZones { get; set; }
    }
}