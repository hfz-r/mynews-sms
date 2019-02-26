using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Devices
{
    public class DeviceModel : BaseEntityModel, ITenantMappingSupportedModel
    {
        public DeviceModel()
        {
            SelectedStoreId = new int();
            AvailableStores = new List<SelectListItem>();

            SelectedTenantIds = new List<int>();
            AvailableTenants = new List<SelectListItem>();
        }

        public string Name { get; set; }

        [Display(Name = "Serial Number")]
        [Required(ErrorMessage = "Serial Number is required")]
        public string SerialNo { get; set; }

        [Display(Name = "Model Number")]
        [Required(ErrorMessage = "Model Number is required")]
        public string ModelNo { get; set; }

        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last activity")]
        public DateTime LastActivityDate { get; set; }

        [Display(Name = "Store")]
        [Required(ErrorMessage = "Store is required")]
        public int SelectedStoreId { get; set; }

        [Display(Name = "Store")]
        //[Required(ErrorMessage = "Store is required")]
        public string StoreName { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public ICollection<Device> Devices { get; set; }

        //multi-tenant setting
        [Display(Name = "Limited to tenants")]
        public IList<int> SelectedTenantIds { get; set; }
        public IList<SelectListItem> AvailableTenants { get; set; }
    }
}
