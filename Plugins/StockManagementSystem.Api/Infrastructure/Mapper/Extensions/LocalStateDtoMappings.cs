using StockManagementSystem.Api.DTOs.Directory;
using StockManagementSystem.Core.Domain.Directory;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class LocalStateDtoMappings
    {
        public static LocalStateDto ToDto(this LocalState state)
        {
            return state.MapTo<LocalState, LocalStateDto>();
        }
    }
}