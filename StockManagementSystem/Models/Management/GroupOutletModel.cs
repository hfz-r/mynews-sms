using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models.Management
{
    public class GroupOutletModel : BaseEntityModel
    {
        public GroupOutletModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [Display(Name = "Group Name")]
        public string GroupName { get; set; }

        [Display(Name = "Store")]
        public string StoreName { get; set; }

        [Display(Name = "Stores")]
        [Required(ErrorMessage = "Store is required")]
        public IList<int> SelectedStoreIds { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last activity")]
        public DateTime LastActivityDate { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public ICollection<StoreGrouping> StoreGroupings { get; set; }
    }
}
