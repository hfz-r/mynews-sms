using NUnit.Framework;
using StockManagementSystem.Api.Infrastructure.Mapper;

namespace Api.Tests
{
    [SetUpFixture]
    public class SetUp
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            ApiMapperConfiguration apiMaps = new ApiMapperConfiguration();
        }
    }
}