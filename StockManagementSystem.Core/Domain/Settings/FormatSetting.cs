using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class FormatSetting : BaseEntity
    {
        public string Format { get; set; }

        public string Prefix { get; set; }

        public string Name { get; set; }

        public int Length { get; set; }
    }
}
