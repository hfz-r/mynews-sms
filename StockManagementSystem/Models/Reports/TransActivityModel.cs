using System;
using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Reports
{
    public class TransActivityModel : BaseEntityModel
    {
        [Display(Name = "Branch")]
        public string Branch { get; set; }

        [Display(Name = "Transaction Category")]
        public string Category { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
    }
}