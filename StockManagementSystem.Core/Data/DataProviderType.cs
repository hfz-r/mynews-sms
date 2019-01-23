using System.Runtime.Serialization;

namespace StockManagementSystem.Core.Data
{
    public enum DataProviderType
    {
        //TODO: other data providers
        [EnumMember(Value = "")]
        Unknown,

        [EnumMember(Value = "sqlserver")]
        SqlServer
    }
}