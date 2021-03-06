﻿using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Management
{
    public class AssignUserSearchModel : BaseSearchModel
    {
        public AssignUserSearchModel()
        {
            SelectedUserStoreId = new List<int>();
            AvailableUserStores = new List<SelectListItem>();
            SelectedUserIds = new List<int>();
            AvailableUsers = new List<SelectListItem>();
        }

        [Display(Name = "Store")]
        public IList<int> SelectedUserStoreId { get; set; }

        public IList<SelectListItem> AvailableUserStores { get; set; }

        [Display(Name = "User")]
        public IList<int> SelectedUserIds { get; set; }

        public IList<SelectListItem> AvailableUsers { get; set; }
    }
}
