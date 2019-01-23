using System;
using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Logging
{
    public class SignedInLogSearchModel : BaseSearchModel
    {
        [Display(Name = "Last sign in from")]
        [UIHint("DateNullable")]
        public DateTime? LastLoginFrom { get; set; }

        [Display(Name = "Last sign in to")]
        [UIHint("DateNullable")]
        public DateTime? LastLoginTo { get; set; }

        [Display(Name = "IP address")]
        public string LastIpAddress { get; set; }
    }
}