using System;
using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Logging
{
    public class LogModel : BaseEntityModel
    {
        [Display(Name = "Log level")]
        public string LogLevel { get; set; }

        [Display(Name = "Short message")]
        public string ShortMessage { get; set; }

        [Display(Name = "Full message")]
        public string FullMessage { get; set; }

        [Display(Name = "IP address")]
        public string IpAddress { get; set; }

        [Display(Name = "User")]
        public int? UserId { get; set; }

        [Display(Name = "User")]
        public string UserEmail { get; set; }

        [Display(Name = "Page URL")]
        public string PageUrl { get; set; }

        [Display(Name = "Referrer URL")]
        public string ReferrerUrl { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }
    }
}