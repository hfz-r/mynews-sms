using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Common
{
    public class SystemInfoModel : BaseModel
    {
        public SystemInfoModel()
        {
            Headers = new List<HeaderModel>();
        }

        [Display(Name = "ASP.NET info")]
        public string AspNetInfo { get; set; }

        [Display(Name = "Is full trust level")]
        public string IsFullTrust { get; set; }

        [Display(Name = "WebApp Version")]
        public string Version { get; set; }

        [Display(Name = "Operating system")]
        public string OperatingSystem { get; set; }

        [Display(Name = "Server local time")]
        public DateTime ServerLocalTime { get; set; }

        [Display(Name = "Server time zone")]
        public string ServerTimeZone { get; set; }

        [Display(Name = "Coordinated Universal Time (UTC)")]
        public DateTime UtcTime { get; set; }

        [Display(Name = "Current user time")]
        public DateTime CurrentUserTime { get; set; }

        [Display(Name = "HTTP_HOST")]
        public string HttpHost { get; set; }

        [Display(Name = "Headers")]
        public IList<HeaderModel> Headers { get; set; }

        public class HeaderModel : BaseModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}