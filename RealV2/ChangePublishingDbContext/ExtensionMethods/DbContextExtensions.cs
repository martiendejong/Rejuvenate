using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public static class DbContextExtensions
    {
        public static IEnumerable<ChangedRelationship> GetChangedRelationships(
            this IDbContextWithSaveEvent context)
        {
            return GetAddedRelationships(context)
                .Union(GetDeletedRelationships(context));
        }

        public static IEnumerable<ChangedRelationship> GetAddedRelationships(
            this IDbContextWithSaveEvent context)
        {
            return GetChangedRelationships(context, EntityState.Added, (e, i) => e.CurrentValues[i]);
        }

        public static IEnumerable<ChangedRelationship> GetDeletedRelationships(
            this IDbContextWithSaveEvent context)
        {
            return GetChangedRelationships(context, EntityState.Deleted, (e, i) => e.OriginalValues[i]);
        }

        private static IEnumerable<ChangedRelationship> GetChangedRelationships(
            this IDbContextWithSaveEvent context,
            EntityState relationshipState,
            Func<ObjectStateEntry, int, object> getValue)
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;

            return objectContext.ObjectStateManager
                                .GetObjectStateEntries(relationshipState)
                                .Where(e => e.State != EntityState.Detached)
                                .Where(e => e.IsRelationship)
                                .Select(
                                        e =>
                                        new ChangedRelationship(relationshipState, objectContext.GetObjectByKey((EntityKey)getValue(e, 0)),
                                                objectContext.GetObjectByKey((EntityKey)getValue(e, 1))));
                                        /*Tuple.Create(
                                                objectContext.GetObjectByKey((EntityKey)getValue(e, 0)),
                                                objectContext.GetObjectByKey((EntityKey)getValue(e, 1)),
                                                relationshipState));*/
        }
    }
}
