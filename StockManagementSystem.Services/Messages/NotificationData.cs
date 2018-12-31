namespace StockManagementSystem.Services.Messages
{
    /// <summary>
    /// Message structure
    /// </summary>
    public struct NotificationData
    {
        public NotificationType Type { get; set; }

        public string Message { get; set; }
    }
}
