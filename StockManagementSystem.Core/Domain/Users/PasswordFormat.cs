﻿namespace StockManagementSystem.Core.Domain.Users
{
    /// <summary>
    /// Password format
    /// </summary>
    public enum PasswordFormat
    {
        /// <summary>
        /// Clear
        /// </summary>
        Clear = 0,

        /// <summary>
        /// Hashed
        /// </summary>
        Hashed = 1,

        /// <summary>
        /// Encrypted
        /// </summary>
        Encrypted = 2
    }
}