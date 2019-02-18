using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Infrastructure;
using Tests;

namespace Core.Tests.Infrastructure
{
    [TestFixture]
    public class TypeFinderTests
    {
        [Test]
        public void TypeFinder_Benchmark_Findings()
        {
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(x => x.ContentRootPath).Returns(Assembly.GetExecutingAssembly().Location);
            hostingEnvironment.Setup(x => x.WebRootPath).Returns(Directory.GetCurrentDirectory());

            CommonHelper.DefaultFileProvider = new FileProviderHelper(hostingEnvironment.Object);
            var finder = new AppDomainTypeFinder();
            var type = finder.FindClassesOfType<ISomeInterface>().ToList();

            type.Count.ShouldEqual(1);

            typeof(ISomeInterface).IsAssignableFrom(type.FirstOrDefault()).ShouldBeTrue();
        }

        public interface ISomeInterface
        {
        }

        public class SomeClass : ISomeInterface
        {
        }
    }
}