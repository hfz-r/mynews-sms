using System;
using System.Runtime.Serialization;

namespace StockManagementSystem.Core
{
    /// <summary>
    /// Represents errors that occur during application execution
    /// </summary>
    [Serializable]
    public class DefaultException : Exception
    {
        public DefaultException()
        {
        }

        public DefaultException(string message) : base(message)
        {
        }

        public DefaultException(string messageFormat, params object[] args) : base(string.Format(messageFormat, args))
        {
        }

        protected DefaultException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DefaultException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}