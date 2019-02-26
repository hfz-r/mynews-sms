using NUnit.Framework;
using StockManagementSystem.Core.Domain.Tenants;
using StockManagementSystem.Services.Tenants;
using Tests;

namespace Services.Tests.Tenants
{
    [TestFixture]
    public class TenantServiceTests : ServiceTest
    {
        private ITenantService _tenantService;

        [SetUp]
        public new void SetUp()
        {
            _tenantService = new TenantService(null, null);
        }

        [Test]
        public void Can_parse_host_values()
        {
            var tenant = new Tenant
            {
                Hosts = "yourtenant.com, www.yourtenant.com, "
            };

            var hosts = _tenantService.ParseHostValues(tenant);
            hosts.Length.ShouldEqual(2);
            hosts[0].ShouldEqual("yourtenant.com");
            hosts[1].ShouldEqual("www.yourtenant.com");
        }

        [Test]
        public void Can_find_host_value()
        {
            var tenant = new Tenant
            {
                Hosts = "yourtenant.com, www.yourtenant.com, "
            };

            _tenantService.ContainsHostValue(tenant, null).ShouldEqual(false);
            _tenantService.ContainsHostValue(tenant, "").ShouldEqual(false);
            _tenantService.ContainsHostValue(tenant, "tenant.com").ShouldEqual(false);
            _tenantService.ContainsHostValue(tenant, "yourtenant.com").ShouldEqual(true);
            _tenantService.ContainsHostValue(tenant, "yourteNanT.com").ShouldEqual(true);
            _tenantService.ContainsHostValue(tenant, "www.yourtenant.com").ShouldEqual(true);
        }
    }
}