using System;
using System.Threading;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Tasks.Scheduling;

namespace StockManagementSystem.Services.Users
{
    /// <summary>
    /// Represents a task for deleting guest users
    /// </summary>
    public class DeleteGuestsTask : IScheduledTask
    {
        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;

        public DeleteGuestsTask(UserSettings userSettings, IUserService userService)
        {
            _userSettings = userSettings;
            _userService = userService;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var olderThanMinutes = _userSettings.DeleteGuestTaskOlderThanMinutes;

            // Default value in case 0 is returned.  0 would effectively disable this service and harm performance.
            olderThanMinutes = olderThanMinutes == 0 ? 1440 : olderThanMinutes;

            await _userService.DeleteGuestUsers(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes));
        }

        public string Schedule => "*/10 * * * *";

        public bool Enabled => false;
    }
}