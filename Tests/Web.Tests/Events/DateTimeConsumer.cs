using System;
using StockManagementSystem.Services.Events;

namespace Web.Tests.Events
{
    public class DateTimeConsumer : IConsumer<DateTime>
    {
        public void HandleEvent(DateTime eventMessage)
        {
            DateTime = eventMessage;
        }

        public static DateTime DateTime { get; set; }
    }
}