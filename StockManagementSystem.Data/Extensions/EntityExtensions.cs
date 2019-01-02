using System;
using StockManagementSystem.Core;

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
            return type.BaseType != null && type.BaseType.BaseType != null &&
                   (type.BaseType.BaseType == typeof(BaseEntity) || type.BaseType.BaseType == typeof(Entity));
        }

        /// <summary>
        /// Get unproxied entity type
        /// </summary>
        public static Type GetUnproxiedEntityType(this BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Type type = null;
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