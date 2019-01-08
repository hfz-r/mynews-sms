using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.OrderLimits
{
    public class OrderLimitViewModel
    {
        public int? OrderLimitId { get; set; }

        [Required]
        [Display(Name = "Percentage")]
        public string Percentage { get; set; }

        [Display(Name = "BranchNo")]
        public int P_BranchNo { get; set; }

        public ICollection<OrderLimit> OrderLimit { get; set; }

        public List<Store> Store { get; set; }
    }
}
