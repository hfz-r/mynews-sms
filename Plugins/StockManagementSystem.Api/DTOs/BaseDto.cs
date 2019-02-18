using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs
{
    /// <summary>
    /// Represent base data transfer object
    /// </summary>
    public abstract class BaseDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}