using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models.Management
{
    public class OutletManagementModel : BaseEntityModel
    {
        [Display(Name = "Branch Name")]
        public string P_Name { get; set; }

        [Display(Name = "Area Code")]
        public int P_BranchNo { get; set; }

        [Display(Name = "User")]
        public string Name { get; set; }

        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
    }
}
