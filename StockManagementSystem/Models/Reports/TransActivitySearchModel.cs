using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Reports
{
    public class TransActivitySearchModel : BaseSearchModel
    {
        public TransActivitySearchModel()
        {
            Branches = new List<SelectListItem>();
        }

        [Display(Name = "Created from")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [Display(Name = "Created to")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [Display(Name = "Branch")]
        public int? BranchId { get; set; }

        [Display(Name = "Branch")]
        public IList<SelectListItem> Branches { get; set; }
    }
}