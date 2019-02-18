using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using StockManagementSystem.Core;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Messages;
using Tests;

namespace Services.Tests.Messages
{
    [TestFixture]
    public class NotificationServiceTests : ServiceTest
    {
        private INotificationService _notificationService;

        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<ILogger> _logger;
        private Mock<ITempDataDictionaryFactory> _tempDataDictionaryFactory;
        private Mock<IWorkContext> _workContext;

        private ITempDataDictionary _dataDictionary;

        [SetUp]
        public new void SetUp()
        {
            var httpContext = new Mock<HttpContext>();
            var tempDataProvider = new Mock<ITempDataProvider>();

            _dataDictionary = new TempDataDictionary(httpContext.Object, tempDataProvider.Object);

            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _logger = new Mock<ILogger>();
            _tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
            _tempDataDictionaryFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(_dataDictionary);
            _workContext = new Mock<IWorkContext>();

            _notificationService = new NotificationService(
                _httpContextAccessor.Object, 
                _tempDataDictionaryFactory.Object, 
                _workContext.Object, 
                _logger.Object);
        }

        private IList<NotificationData> DeserializedDataDictionary =>
            JsonConvert.DeserializeObject<IList<NotificationData>>(_dataDictionary[MessageDefaults.NotificationListKey]
                .ToString());

        [Test]
        public void Can_add_notification()
        {
            _notificationService.SuccessNotification("success");
            _notificationService.WarningNotification("warning");
            _notificationService.ErrorNotification("error");

            var messageList = DeserializedDataDictionary;
            messageList.Count.ShouldEqual(3);

            var successMsg = messageList.First(m => m.Type == NotificationType.Success);
            successMsg.Message.ShouldEqual("success");

            var warningMsg = messageList.First(m => m.Type == NotificationType.Warning);
            warningMsg.Message.ShouldEqual("warning");

            var errorMsg = messageList.First(m => m.Type == NotificationType.Error);
            errorMsg.Message.ShouldEqual("error");
        }

        [Test]
        public void Can_add_notification_from_exception()
        {
            _notificationService.ErrorNotification(new Exception("error"));

            var exceptionMsg = DeserializedDataDictionary.First();
            exceptionMsg.Message.ShouldEqual("error");
        }
    }
}