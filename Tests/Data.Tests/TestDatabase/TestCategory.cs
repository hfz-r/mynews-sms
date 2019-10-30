using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Core;

namespace Data.Tests.TestDatabase
{
    public class TestCategory : BaseEntity
    {
        public TestCategory()
        {
            Products = new HashSet<TestProduct>();
        }

        [Key]
        public new int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<TestProduct> Products { get; set; }
    }
}