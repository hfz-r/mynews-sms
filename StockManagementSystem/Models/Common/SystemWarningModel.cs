using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Common
{
    public class SystemWarningModel : BaseModel
    {
        public SystemWarningLevel Level { get; set; }

        public string Text { get; set; }

        public bool DontEncode { get; set; }
    }

    public enum SystemWarningLevel
    {
        Pass,
        Recommendation,
        Warning,
        Fail
    }
}