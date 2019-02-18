using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Plugins;

namespace Services.Tests
{
    [TestFixture]
    public abstract class ServiceTest
    {
        [SetUp]
        public void SetUp()
        {
            //init plugins
            InitPlugins();
        }

        private void InitPlugins()
        {
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
            hostingEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());

            CommonHelper.DefaultFileProvider = new FileProviderHelper(hostingEnvironment.Object);

            //TODO - plugin test
            Singleton<PluginsInfo>.Instance = new PluginsInfo
            {
                PluginDescriptors = new List<PluginDescriptor>
                {
                }
            };
        }
    }
}