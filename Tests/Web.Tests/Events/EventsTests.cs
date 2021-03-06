﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Events;
using Tests;

namespace Web.Tests.Events
{
    [TestFixture]
    public class EventsTests
    {
        private IEventPublisher _eventPublisher;

        [OneTimeSetUp]
        public void SetUp()
        {
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
            hostingEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            CommonHelper.DefaultFileProvider = new FileProviderHelper(hostingEnvironment.Object);

            var engine = new Mock<Engine>();
            var serviceProvider = new TestServiceProvider();
            engine.Setup(x => x.ServiceProvider).Returns(serviceProvider);
            engine.Setup(x => x.ResolveAll<IConsumer<DateTime>>()).Returns(new List<IConsumer<DateTime>> { new DateTimeConsumer() });
            EngineContext.Replace(engine.Object);
            _eventPublisher = new EventPublisher();
        }

        [Test]
        public void Can_publish_event()
        {
            var oldDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(7));
            DateTimeConsumer.DateTime = oldDateTime;

            var newDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(5));
            _eventPublisher.Publish(newDateTime);
            Assert.AreEqual(DateTimeConsumer.DateTime, newDateTime);
        }
    }
}