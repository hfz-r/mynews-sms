namespace StockManagementSystem.Services.Installation
{
    public interface IInstallationService
    {
        void InstallData(string defaultUserEmail, string defaultUsername, string defaultUserPassword, bool installSampleData = true);
    }
}