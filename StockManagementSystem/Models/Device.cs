using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models
{
    public class Device
    {
        [Key]
        public int ID { get; set; }

        public string SerialNo { get; set; }

        public string ModelNo { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public string TokenID { get; set; }

        public string StoreID { get; set; }

        public byte Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
