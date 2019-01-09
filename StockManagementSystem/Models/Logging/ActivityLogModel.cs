using System;
using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Logging
{
    public class ActivityLogModel : BaseEntityModel
    {
        [Display(Name = "Activity log type")]
        public string ActivityLogTypeName { get; set; }

        [Display(Name = "User")]
        public int UserId { get; set; }

        [Display(Name = "User")]
        public string UserEmail { get; set; }

        public string EntityName { get; set; }

        [Display(Name = "Message")]
        public string Comment { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "IP address")]
        public string IpAddress { get; set; }
    }
}