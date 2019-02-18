using System.Data.Common;

namespace StockManagementSystem.Core.Data
{
    public interface IDataProvider
    {
        void InitializeDatabase();

        DbParameter GetParameter();

        bool BackupSupported { get; }

        int SupportedLengthOfBinaryHash { get; }
    }
}