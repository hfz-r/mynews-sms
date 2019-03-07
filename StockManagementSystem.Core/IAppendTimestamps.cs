using System;

namespace StockManagementSystem.Core
{
    /// <summary>
    /// Append entity with timestamp trailing
    /// </summary>
    public interface IAppendTimestamps
    {
        DateTime CreatedOnUtc { get; set; }

        DateTime? ModifiedOnUtc { get; set; }
    }
}