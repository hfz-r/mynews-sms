using NUnit.Framework;
using StockManagementSystem.Core;
using Tests;

namespace Core.Tests
{
    [TestFixture]
    public class CommonHelperTests
    {
        [Test]
        public void Can_get_typed_value()
        {
            CommonHelper.To<int>("1000").ShouldBe<int>();
            CommonHelper.To<int>("1000").ShouldEqual(1000);
        }
    }
}