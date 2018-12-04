using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models
{
    public class Roles
    {
        private static readonly string[] roles = { Admin };

        public const string Admin = "Admin";

        public static IEnumerable<string> All
        {
            get
            {
                return roles;
            }
        }
    }
}
