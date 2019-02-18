using NUnit.Framework;
using StockManagementSystem.Core;
using Tests;

namespace Core.Tests
{
    [TestFixture]
    public class CommonHelperIpAddressValidatorTests
    {
        [Test]
        public void Text_is_valid_ipv4_address()
        {
            var ip = "123.123.123.123";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.ShouldEqual(true);
        }

        [Test]
        public void Text_is_valid_ipv6_address()
        {
            var ip = "FE80:0000:0000:0000:0202:B3FF:FE1E:8329";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.ShouldEqual(true);
        }

        [Test]
        public void Text_is_not_valid_ip_address()
        {
            var ip = "abc";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.ShouldEqual(false);
        }

        [Test]
        public void Text_is_valid_ipv_address_with_wrong_range()
        {
            var ip = "999.999.999.999";
            var result = CommonHelper.IsValidIpAddress(ip);
            result.ShouldEqual(false);
        }
    }
}