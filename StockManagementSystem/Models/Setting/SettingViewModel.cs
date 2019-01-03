using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models.Setting
{
    public class SettingViewModel
    {
        public int? OrderLimitId { get; set; }

        [Required(ErrorMessage = "Percentage is required.")]
        [Display(Name = "Percentage")]
        public double Percentage { get; set; }

        [Display(Name = "BranchNo")]
        public int P_BranchNo { get; set; }

        public ICollection<OrderLimit> OrderLimit { get; set; }

        public List<Store> Store { get; set; }
    }
}
