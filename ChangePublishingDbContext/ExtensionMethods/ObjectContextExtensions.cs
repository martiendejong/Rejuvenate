using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace Rejuvenate
{
    public static class ObjectContextExtensions
    {
        public static IEnumerable<EntityRelationChange> GetRelationChanges(this ObjectContext objectContext, EntityState relationshipState, Func<ObjectStateEntry, int, object> getValue)
        {
            return objectContext.GetRelationChangeEntries(relationshipState)
                .Select(entry => objectContext.GetRelationChange(entry, relationshipState, getValue));
        }

        public static IEnumerable<ObjectStateEntry> GetRelationChangeEntries(this ObjectContext objectContext, EntityState relationshipState)
        {
            return objectContext.ObjectStateManager.GetObjectStateEntries(relationshipState)
                .Where(e => e.State != EntityState.Detached)
                .Where(e => e.IsRelationship);
        }

        public static EntityRelationChange GetRelationChange(this ObjectContext objectContext, ObjectStateEntry entry, EntityState relationshipState, Func<ObjectStateEntry, int, object> getValue)
        {            
            return new EntityRelationChange(relationshipState, objectContext.GetEntity(GetEntityKey(entry, getValue, 0)), objectContext.GetEntity(GetEntityKey(entry, getValue, 1)));
        }

        public static object GetEntity(this ObjectContext objectContext, EntityKey key)
        {
            return objectContext.GetObjectByKey(key);
        }

        public static EntityKey GetEntityKey(ObjectStateEntry entry, Func<ObjectStateEntry, int, object> getValue, int index)
        {
            return (EntityKey)getValue(entry, index);
        }
    }
}
