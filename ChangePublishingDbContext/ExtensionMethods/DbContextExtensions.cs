using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public static class DbContextExtensions
    {
        public static IEnumerable<EntityRelationChange> GetChangedRelationships(
            this IDbContextWithSaveEvent context)
        {
            return GetAddedRelationships(context)
                .Union(GetDeletedRelationships(context));
        }

        public static IEnumerable<EntityRelationChange> GetAddedRelationships(
            this IDbContextWithSaveEvent context)
        {
            return GetChangedRelationships(context, EntityState.Added, (e, i) => e.CurrentValues[i]);
        }

        public static IEnumerable<EntityRelationChange> GetDeletedRelationships(
            this IDbContextWithSaveEvent context)
        {
            return GetChangedRelationships(context, EntityState.Deleted, (e, i) => e.OriginalValues[i]);
        }

        private static IEnumerable<EntityRelationChange> GetChangedRelationships(
            this IDbContextWithSaveEvent context,
            EntityState relationshipState,
            Func<ObjectStateEntry, int, object> getValue)
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            return objectContext.GetRelationChanges(relationshipState, getValue);
        }

        public static T GetExistingEntity<T>(this IDbContextWithSaveEvent context, string entitySetName, T entity)
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            ObjectStateEntry entry;
            objectContext.ObjectStateManager.TryGetObjectStateEntry(objectContext.CreateEntityKey(entitySetName, entity), out entry);
            return (T)entry.Entity;
        }

        public static void AttachToOrGet<T>(this IDbContextWithSaveEvent context, string entitySetName, ref T entity)
        {
            var existingEntity = context.GetExistingEntity(entitySetName, entity);
            if (existingEntity == null)
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                objectContext.AttachTo(entitySetName, entity);
            }
        }
    }
}
