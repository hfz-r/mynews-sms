using System;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Messages;

namespace Api.Tests.ConvertersTests
{
    public class TestingObject
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
        public DateTime? DateTimeNullableProperty { get; set; }
        public bool? BooleanNullableStatusProperty { get; set; }
        public PasswordFormat? PasswordFormat { get; set; }
        public NotificationType? NotificationType { get; set; }
    }
}