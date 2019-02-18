using Moq;
using NUnit.Framework;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Infrastructure;
using Tests;

namespace Web.Tests.Validators
{
    [TestFixture]
    public class BaseValidatorTests
    {
        protected Mock<IWorkContext> WorkContext;

        [SetUp]
        public void Setup()
        {
            var engine = new Mock<Engine>();
            var serviceProvider = new TestServiceProvider();

            engine.Setup(x => x.ServiceProvider).Returns(serviceProvider);
            //engine.Setup(x => x.ResolveUnregistered(typeof()));
            EngineContext.Replace(engine.Object);
        }
    }
}