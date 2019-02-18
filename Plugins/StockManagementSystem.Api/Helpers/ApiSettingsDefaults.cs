namespace StockManagementSystem.Api.Helpers
{
    public static class ApiSettingsDefaults
    {
        /// <summary>
        /// Api signing key
        /// </summary>
        public static string TokenSigningKeyFileName => "api-token-signing-key.json";

        /// <summary>
        /// Script to upgrade api data
        /// </summary>
        public static string ApiUpgradeScript => "upgrade_script.sql";
    }
}