using System;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Logging
{
    public class SignedInLogModel : BaseEntityModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string LastIpAddress { get; set; }

        public DateTime LastLoginDate { get; set; }
    }
}