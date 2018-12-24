using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models.Devices
{
    public class DeviceViewModel
    {
        public int? DeviceId { get; set; }

        [Required]
        [Display(Name = "Serial Number")]
        public string SerialNo { get; set; }

        [Required]
        [Display(Name = "Model Number")]
        public string ModelNo { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
                
        [Display(Name = "BranchNo")]
        public int P_BranchNo { get; set; }

        public ICollection<Device> Device { get; set; }

        public List<Store> Store { get; set; }
    }
}
