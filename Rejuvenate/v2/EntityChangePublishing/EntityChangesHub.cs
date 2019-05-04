using EFExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2.EntityChangePublishing
{
    public class EntityChangesHub<EntityType> : IEntityChangesHub where EntityType : class, new()
    {
        public List<EntitiesChangedHandler<EntityType>> Handlers = new List<EntitiesChangedHandler<EntityType>>();

        public IEnumerable<EntityChangeMessage<EntityType>> GatheredChanges;

        public void Add(object handler)
        {
            Handlers.Add((EntitiesChangedHandler<EntityType>)handler);
        }

        public void GatherChanges(DbChangeTracker changeTracker, IEnumerable<Tuple<object, object, EntityState>> relations)
        {
            var entries = changeTracker.Entries<EntityType>();
            GatheredChanges = entries.Select(GetEntityChangedMessage).ToList().Where(m => m != null);
            var relationsOfEntityType = relations.Where(relation => relation.Item1 as EntityType != null);
            GatheredChanges = GatheredChanges.Union(relationsOfEntityType.Select(GetRelationshipChangedMessage));
        }

        public void PublishChanges()
        {
            Handlers.ForEach(h => h(GatheredChanges));
        }

        public static EntityChangeMessage<EntityType> GetEntityChangedMessage(DbEntityEntry<EntityType> entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    return GetEntityAddedMessage(entry);
                case EntityState.Deleted:
                    return GetEntityRemovedMessage(entry);
                case EntityState.Modified:
                    return GetEntityModifiedMessage(entry);
                default:
                    return null;
            }
        }

        public static EntityChangeMessage<EntityType> GetEntityAddedMessage(DbEntityEntry<EntityType> entry)
        {
            return new EntityChangeMessage<EntityType>(entry.State, null, entry.Entity);
        }

        public static EntityChangeMessage<EntityType> GetEntityRemovedMessage(DbEntityEntry<EntityType> entry)
        {
            return new EntityChangeMessage<EntityType>(entry.State, entry.Entity, null);
        }

        public static EntityChangeMessage<EntityType> GetEntityModifiedMessage(DbEntityEntry<EntityType> entry)
        {
            return new EntityChangeMessage<EntityType>(entry.State, entry.OriginalValues.ToEntity<EntityType>(), entry.Entity);
        }

        public static EntityChangeMessage<EntityType> GetRelationshipChangedMessage(Tuple<object, object, EntityState> tuple)
        {
            switch(tuple.Item3)
            {
                case EntityState.Added:
                    return GetRelationshipAddedMessage(tuple);
                case EntityState.Deleted:
                    return GetRelationshipDeletedMessage(tuple);
                default:
                    throw new Exception("Only added or deleted relationships are supported");
            }
        }

        public static EntityChangeMessage<EntityType> GetRelationshipAddedMessage(Tuple<object, object, EntityState> tuple)
        {
            return new EntityChangeMessage<EntityType>(EntityState.Modified, tuple.Item1 as EntityType, tuple.Item1 as EntityType);
        }

        public static EntityChangeMessage<EntityType> GetRelationshipDeletedMessage(Tuple<object, object, EntityState> tuple)
        {//hier nog iets mee doen
            return new EntityChangeMessage<EntityType>(EntityState.Modified, tuple.Item1 as EntityType, tuple.Item1 as EntityType);
        }
    }
}
