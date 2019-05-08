using Newtonsoft.Json;
using StockManagementSystem.Api.DTOs;

namespace StockManagementSystem.Api.Json.Serializer
{
    public interface IJsonFieldsSerializer
    {
        string Serialize(ISerializableObject objectToSerialize, string jsonFields, JsonSerializer serializer = null);
    }
}