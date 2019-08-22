using AutoMapper;
using NUnit.Framework;
using StockManagementSystem.Api.Infrastructure.Mapper;

namespace Api.Tests.Infrastructure
{
    [TestFixture]
    public class AutoMapperConfigurationTest
    {
        [Test]
        public void Configuration_is_valid()
        {
            var config = AutoMapperApiConfiguration.MapperConfigurationExpression;

            var mapperConfiguration = new MapperConfiguration(config);
            mapperConfiguration.AssertConfigurationIsValid();
        }
    }
}