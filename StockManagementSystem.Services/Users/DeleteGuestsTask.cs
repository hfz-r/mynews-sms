using System;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Tasks;

namespace StockManagementSystem.Services.Users
{
    /// <summary>
    /// Represents a task for deleting guest users
    /// </summary>
    public class DeleteGuestsTask : IScheduleTask
    {
        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;

        public DeleteGuestsTask(UserSettings userSettings, IUserService userService)
        {
            _userSettings = userSettings;
            _userService = userService;
        }

        public void Execute()
        {
            var olderThanMinutes = _userSettings.DeleteGuestTaskOlderThanMinutes;

            // Default value in case 0 is returned.  0 would effectively disable this service and harm performance.
            olderThanMinutes = olderThanMinutes == 0 ? 1440 : olderThanMinutes;

            _userService.DeleteGuestUsers(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes)).GetAwaiter().GetResult();
        }
    }
}