using System.Collections.Generic;
using System.Linq;

namespace StockManagementSystem.Services.Users
{
    /// <summary>
    /// Change password result
    /// </summary>
    public class ChangePasswordResult
    {
        public ChangePasswordResult()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success => !Errors.Any();

        public IList<string> Errors { get; set; }

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}