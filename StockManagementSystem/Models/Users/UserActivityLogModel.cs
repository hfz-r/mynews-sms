using System;
using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Users
{
    public class UserActivityLogModel : BaseEntityModel
    {
        [Display(Name = "Activity Log Type")]
        public string ActivityLogTypeName { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "IP address")]
        public string IpAddress { get; set; }
    }
}