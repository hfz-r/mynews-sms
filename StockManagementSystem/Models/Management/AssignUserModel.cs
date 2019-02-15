using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Management
{
    public class AssignUserModel : BaseEntityModel
    {
        public AssignUserModel()
        {
            SelectedStoreId = new int();
            AvailableStores = new List<SelectListItem>();
            SelectedUserIds = new List<int>();
            AvailableUsers = new List<SelectListItem>();
        }

        [Display(Name = "Store")]
        [Required(ErrorMessage = "Store is required")]
        public int SelectedStoreId { get; set; }

        [Display(Name = "Store")]
        [Required(ErrorMessage = "Store is required")]
        public string StoreName { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        [Display(Name = "User")]
        public string User { get; set; }

        [Display(Name = "User")]
        public IList<int> SelectedUserIds { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last activity")]
        public DateTime LastActivityDate { get; set; }

        public ICollection<StoreUserAssign> StoreUsers { get; set; }

        public ICollection<StoreUserAssignStores> StoreUserAssignStore { get; set; }
    }
}
