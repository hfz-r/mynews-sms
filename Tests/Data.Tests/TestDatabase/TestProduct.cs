using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StockManagementSystem.Core;

namespace Data.Tests.TestDatabase
{
    public class TestProduct : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public new int Id { get; set; }

        public string Name { get; set; }

        public TestCategory Category { get; set; }

        public int CategoryId { get; set; }

        public int Stock { get; set; }

        public bool? InStock { get; set; }
    }
}