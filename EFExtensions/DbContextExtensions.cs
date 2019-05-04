using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFExtensions
{
    public static class DbContextExtensions
    {
        public static IEnumerable<Tuple<object, object, EntityState>> GetRelationships(
            this DbContext context)
        {
            return GetAddedRelationships(context)
                    .Union(GetDeletedRelationships(context));
        }

        public static IEnumerable<Tuple<object, object, EntityState>> GetAddedRelationships(
            this DbContext context)
        {
            return GetRelationships(context, EntityState.Added, (e, i) => e.CurrentValues[i]);
        }

        public static IEnumerable<Tuple<object, object, EntityState>> GetDeletedRelationships(
            this DbContext context)
        {
            return GetRelationships(context, EntityState.Deleted, (e, i) => e.OriginalValues[i]);
        }

        private static IEnumerable<Tuple<object, object, EntityState>> GetRelationships(
            this DbContext context,
            EntityState relationshipState,
            Func<ObjectStateEntry, int, object> getValue)
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;

            return objectContext.ObjectStateManager
                                .GetObjectStateEntries(relationshipState)
                                .Where(e => e.State != EntityState.Detached)
                                .Where(e => e.IsRelationship)
                                .Select(
                                        e => Tuple.Create(
                                                objectContext.GetObjectByKey((EntityKey)getValue(e, 0)),
                                                objectContext.GetObjectByKey((EntityKey)getValue(e, 1)), 
                                                relationshipState));
        }
    }
}
