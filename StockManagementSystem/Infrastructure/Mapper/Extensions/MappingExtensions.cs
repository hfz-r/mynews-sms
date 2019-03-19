﻿using System;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Configuration;
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

        #region Model-Entity mapping

        public static TModel ToModel<TModel>(this BaseEntity entity) where TModel : BaseEntityModel
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return entity.Map<TModel>();
        }

        public static TModel ToModel<TEntity, TModel>(this TEntity entity, TModel model)
            where TEntity : BaseEntity where TModel : BaseEntityModel
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

        #endregion

        #region Model-Settings mapping

        public static TModel ToSettingsModel<TModel>(this ISettings settings) where TModel : BaseModel, ISettingsModel
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return settings.Map<TModel>();
        }

        public static TSettings ToSettings<TSettings, TModel>(this TModel model, TSettings settings) 
            where TSettings : class, ISettings where TModel : BaseModel, ISettingsModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return model.MapTo(settings);
        }

        #endregion
    }
}