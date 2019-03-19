namespace StockManagementSystem.Api.Constants
{
    public static class WebHookNames
    {
        public const string FiltersGetAction = "FiltersGetAction";

        public const string GetWebhookByIdAction = "GetWebHookByIdAction";

        public const string UsersCreated = "users/created";
        public const string UsersUpdated = "users/updated";
        public const string UsersDeleted = "users/deleted";

        public const string DevicesCreated = "devices/created";
        public const string DevicesUpdated = "devices/updated";
        public const string DevicesDeleted = "devices/deleted";

        public const string ItemsCreated = "items/created";
        public const string ItemsUpdated = "items/updated";
        public const string ItemsDeleted = "items/deleted";
    }
}