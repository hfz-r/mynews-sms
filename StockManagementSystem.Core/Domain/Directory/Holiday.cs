using System;
using System.Globalization;

namespace StockManagementSystem.Core.Domain.Directory
{
    public class Holiday : BaseEntity
    {
        private DateTime _fullDateTime;

        public int Year { get; set; }

        public string Date { get; set; }

        public string Day { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public DateTime FullDateTime
        {
            get => _fullDateTime;
            set
            {
                _fullDateTime = value;

                Year = _fullDateTime.Year;
                Date = _fullDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                Day = _fullDateTime.ToString("ddd");
            }
        }

        public virtual State State { get; set; }
    }
}