using System;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;

namespace StockManagementSystem.Data.Extensions
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Check whether an entity is proxy
        /// </summary>
        private static bool IsProxy(this BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var type = entity.GetType();
            return type.BaseType != null && type.BaseType.BaseType != null && type.Name.Contains("Proxy") &&
                   type.BaseType.BaseType == typeof(BaseEntity);
        }

        /// <summary>
        /// Get unproxied entity type
        /// </summary>
        public static Type GetUnproxiedEntityType(this BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Type type = null;
            
            //cachable entity (get the base entity type)
            if (entity is IEntityForCaching)
                type = ((IEntityForCaching)entity).GetType().BaseType;

            //EF proxy
            if (entity.IsProxy())
                type = entity.GetType().BaseType;
            else
                type = entity.GetType();

            if (type == null)
                throw new Exception("Original entity type cannot be loaded");

            return type;
        }
    }
}