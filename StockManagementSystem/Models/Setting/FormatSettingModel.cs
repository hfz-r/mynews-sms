using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models.Setting
{
    public class FormatSettingModel : BaseEntityModel
    {
        [Display(Name = "Format")]
        public string Format { get; set; }

        [Display(Name = "Prefix")]
        public string Prefix { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Length")]
        public int Length { get; set; }

        public ICollection<FormatSetting> FormatSettings { get; set; }
    }
}
