using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using StockManagementSystem.Core;
using StockManagementSystem.Services.Logging;

namespace StockManagementSystem.Services.Messages
{
    public class NotificationService : INotificationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        public NotificationService(
            IHttpContextAccessor httpContextAccessor,
            ITempDataDictionaryFactory tempDataDictionaryFactory,
            IWorkContext workContext,
            ILogger logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _workContext = workContext;
            _logger = logger;
        }


        /// <summary>
        /// Save message into TempData
        /// </summary>
        protected virtual void PrepareTempData(HttpContext context, NotificationType type, string message)
        {
            var tempData = _tempDataDictionaryFactory.GetTempData(context);

            //Messages have stored in a serialized list
            var messageList = tempData.ContainsKey(MessageDefaults.NotificationListKey)
                ? JsonConvert.DeserializeObject<IList<NotificationData>>(tempData[MessageDefaults.NotificationListKey]
                    .ToString())
                : new List<NotificationData>();

            messageList.Add(new NotificationData {Type = type, Message = message});

            tempData[MessageDefaults.NotificationListKey] = JsonConvert.SerializeObject(messageList);
        }

        /// <summary>
        /// Log exception
        /// </summary>
        protected virtual void LogException(Exception exception)
        {
            if (exception == null)
                return;

            var user = _workContext.CurrentUser;
            _logger.Error(exception.Message, exception, user);
        }

        /// <summary>
        /// Display success notification
        /// </summary>
        public virtual void SuccessNotification(string message, HttpContext context = null)
        {
            PrepareTempData(context ?? _httpContextAccessor.HttpContext, NotificationType.Success, message);
        }

        /// <summary>
        /// Display warning notification
        /// </summary>
        public virtual void WarningNotification(string message, HttpContext context = null)
        {
            PrepareTempData(context ?? _httpContextAccessor.HttpContext, NotificationType.Warning, message);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="context">HttpContext</param>
        public virtual void ErrorNotification(string message, HttpContext context = null)
        {
            PrepareTempData(context ?? _httpContextAccessor.HttpContext, NotificationType.Error, message);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        /// <param name="context">HttpContext</param>
        public virtual void ErrorNotification(Exception exception, bool logException = true, HttpContext context = null)
        {
            if (exception == null)
                return;

            if (logException)
                LogException(exception);

            ErrorNotification(exception.Message, context);
        }
    }
}