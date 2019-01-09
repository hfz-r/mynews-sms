using AutoMapper;
using NUnit.Framework;
using StockManagementSystem.Core.Infrastructure.Mapper;
using StockManagementSystem.Infrastructure.Mapper;

namespace Web.Tests.Infrastructure
{
    [TestFixture]
    public class AutoMapperConfigurationTest
    {
        [Test]
        public void Configuration_is_valid()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(DefaultMapperConfiguration));
            });

            AutoMapperConfiguration.Init(config);
            AutoMapperConfiguration.MapperConfiguration.AssertConfigurationIsValid();
        }
    }
}