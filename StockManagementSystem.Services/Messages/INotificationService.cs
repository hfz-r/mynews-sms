using System;
using Microsoft.AspNetCore.Http;

namespace StockManagementSystem.Services.Messages
{
    public interface INotificationService
    {
        void ErrorNotification(Exception exception, bool logException = true, HttpContext context = null);
        void ErrorNotification(string message, HttpContext context = null);
        void SuccessNotification(string message, HttpContext context = null);
        void WarningNotification(string message, HttpContext context = null);
    }
}