using AutoMapper;
using AutoMapper.Configuration;

namespace StockManagementSystem.Api.Infrastructure.Mapper
{
    public static class AutoMapperApiConfiguration
    {
        private static MapperConfigurationExpression _mapperConfigurationExpression;
        private static IMapper _mapper;
        private static readonly object _lockObject = new object();

        #region Public fields

        public static MapperConfigurationExpression MapperConfigurationExpression =>
            _mapperConfigurationExpression ?? (_mapperConfigurationExpression = new MapperConfigurationExpression());

        public static IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    lock (_lockObject)
                    {
                        if (_mapper == null)
                        {
                            var mapperConfiguration = new MapperConfiguration(MapperConfigurationExpression);
                            _mapper = mapperConfiguration.CreateMapper();
                        }
                    }
                }

                return _mapper;
            }
        }

        #endregion

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }
    }
}