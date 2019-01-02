using System;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Infrastructure.Mapper;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Infrastructure.Mapper.Extensions
{
    /// <summary>
    /// Represents the extensions to map entity to model and vise versa
    /// </summary>
    public static class MappingExtensions
    {
        #region Utilities

        private static TDestination Map<TDestination>(this object source)
        {
            return AutoMapperConfiguration.Mapper.Map<TDestination>(source);
        }

        private static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        #endregion

        public static TModel ToModel<TModel>(this BaseEntity entity) where TModel : BaseEntityModel
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return entity.Map<TModel>();
        }

        public static TModel ToModel<TEntity, TModel>(this TEntity entity, TModel model)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return entity.MapTo(model);
        }

        public static TEntity ToEntity<TEntity>(this BaseEntityModel model) where TEntity : BaseEntity
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return model.Map<TEntity>();
        }

        public static TEntity ToEntity<TEntity, TModel>(this TModel model, TEntity entity)
            where TEntity : BaseEntity where TModel : BaseEntityModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return model.MapTo(entity);
        }
    }
}