using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Directory
{
    public class LocalState : BaseEntity
    {
        private ICollection<Holiday> _holidays;

        public string Abbreviation { get; set; }

        public string Description { get; set; }

        public bool? IsSameWeekend { get; set; }

        public virtual ICollection<Holiday> Holidays
        {
            get => _holidays ?? (_holidays = new List<Holiday>());
            set => _holidays = value;
        }
    }
}