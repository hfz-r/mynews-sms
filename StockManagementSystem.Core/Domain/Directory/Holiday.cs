using System;
using System.Globalization;

namespace StockManagementSystem.Core.Domain.Directory
{
    public class Holiday : BaseEntity
    {
        //private DateTime _fullDateTime;

        public int Year { get; set; }

        public string Date { get; set; }

        public string Day { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string State { get; set; }

        //public DateTime FullDateTime
        //{
        //    get => _fullDateTime;
        //    set
        //    {
        //        _fullDateTime = value;

        //        P_Year = _fullDateTime.Year;
        //        P_Date = _fullDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        //        P_Day = _fullDateTime.ToString("ddd");
        //    }
        //}

        //public virtual LocalState State { get; set; }
    }
}