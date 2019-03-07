using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Replenishments
{
    public class ReplenishmentModel : BaseEntityModel
    {
        public ReplenishmentModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        public string Name { get; set; }

        [Display(Name = "Buffer Days")]
        [Required(ErrorMessage = "Buffer Days is required")]
        [DataType("Integer")]
        public int BufferDays { get; set; }

        [Display(Name = "Replenishment Quantity")]
        [Required(ErrorMessage = "Replenishment Quantity is required")]
        [DataType("Integer")]
        public int ReplenishmentQty { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last activity")]
        public DateTime LastActivityDate { get; set; }

        [Display(Name = "Store")]
        public string StoreName { get; set; }

        [Display(Name = "Stores")]
        [Required(ErrorMessage = "Store is required")]
        public IList<int> SelectedStoreIds { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public ICollection<Replenishment> Replenishments { get; set; }
    }
}
