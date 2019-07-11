namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class GenericDtoMappings
    {
        public static T ToDto<T, E>(this E entity)
        {
            return entity.MapTo<E, T>();
        }
    }
}