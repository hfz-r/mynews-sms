using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models.System
{
    public class ServerViewModel
    {
        [Display(Name = "App Version")]
        public string AppVersion { set; get; }
    }
}
