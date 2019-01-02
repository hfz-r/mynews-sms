using Microsoft.AspNetCore.Mvc;

namespace StockManagementSystem.Web.Mvc
{
    /// <summary>
    /// Null JSON result
    /// </summary>
    public class NullJsonResult : JsonResult
    {
        public NullJsonResult() : base(null)
        {
        }
    }
}