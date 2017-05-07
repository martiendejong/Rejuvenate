using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFExtensions
{
    public static class DbPropertyValuesCreateEntityExtensionMethods
    {
        /*
         * Creates an entity from the given values
         */
        public static EntityType ToEntity<EntityType>(this DbPropertyValues values) where EntityType : class, new()
        {
            EntityType entity = new EntityType();
            values.ProjectOn(entity);
            return entity;
        }

        /*
         * Fill an entity with the given values
         */
        public static EntityType ProjectOn<EntityType>(this DbPropertyValues values, EntityType entity)
        {
            Type type = typeof(EntityType);

            foreach (var name in values.PropertyNames)
            {
                var property = type.GetProperty(name);
                property.SetValue(entity, values.GetValue<object>(name));
            }

            return entity;
        }
    }
}