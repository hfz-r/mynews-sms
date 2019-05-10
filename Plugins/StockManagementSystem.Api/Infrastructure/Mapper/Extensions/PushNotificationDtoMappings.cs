using StockManagementSystem.Api.DTOs.PushNotifications;
using StockManagementSystem.Core.Domain.PushNotifications;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class PushNotificationDtoMappings
    {
        public static PushNotificationDto ToDto(this PushNotification pushNotification)
        {
            return pushNotification.MapTo<PushNotification, PushNotificationDto>();
        }
    }
}