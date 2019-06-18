using System.Collections.Generic;
using StockManagementSystem.Api.DTOs;

namespace StockManagementSystem.Api.Models.GenericsParameters
{
    public class SearchWrapper<T> where T : BaseDto
    {
        public int CountResult { get; set; }

        public IList<T> ListResult { get; set; }
    }
}