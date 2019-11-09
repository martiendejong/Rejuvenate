using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext.Implementation
{
    public class ChangeTracker<EntityType> : IChangesCollector, IChangeTracker<EntityType> where EntityType : class, new()
    {
        public event EntitiesChangedHandler<EntityType> EntitiesChanged;

        public void CollectChanges(DbChangeTracker changeTracker, IEnumerable<ChangedRelationship> relations)
        {
            var entries = changeTracker.Entries<EntityType>();
            CollectedChanges = entries.Select(GetEntityChangedMessage).ToList().Where(m => m != null);
            var relationsOfEntityType = relations.Where(relation => relation.Parent as EntityType != null);
            CollectedChanges = CollectedChanges.Union(relationsOfEntityType.Select(GetRelationshipChangedMessage));
        }

        public void PublishChanges()
        {
            if (CollectedChanges.Any())
            {
                EntitiesChanged?.Invoke(CollectedChanges);
            }
        }

        protected IEnumerable<EntityChange<EntityType>> CollectedChanges;

        protected static EntityChange<EntityType> GetEntityChangedMessage(DbEntityEntry<EntityType> entry)
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

        protected static EntityChange<EntityType> GetEntityAddedMessage(DbEntityEntry<EntityType> entry)
        {
            return new EntityChange<EntityType>(entry.State, null, entry.Entity);
        }

        protected static EntityChange<EntityType> GetEntityRemovedMessage(DbEntityEntry<EntityType> entry)
        {
            return new EntityChange<EntityType>(entry.State, entry.Entity, null);
        }

        protected static EntityChange<EntityType> GetEntityModifiedMessage(DbEntityEntry<EntityType> entry)
        {
            return new EntityChange<EntityType>(entry.State, entry.OriginalValues.ToEntity<EntityType>(), entry.Entity);
        }

        protected static EntityChange<EntityType> GetRelationshipChangedMessage(ChangedRelationship tuple)
        {
            switch (tuple.State)
            {
                case EntityState.Added:
                    return GetRelationshipAddedMessage(tuple);
                case EntityState.Deleted:
                    return GetRelationshipDeletedMessage(tuple);
                default:
                    throw new Exception("Only added or deleted relationships are supported");
            }
        }

        protected static EntityChange<EntityType> GetRelationshipAddedMessage(ChangedRelationship tuple)
        {
            return new EntityChange<EntityType>(EntityState.Modified, tuple.Parent as EntityType, tuple.Parent as EntityType);
        }

        protected static EntityChange<EntityType> GetRelationshipDeletedMessage(ChangedRelationship tuple)
        {//hier nog iets mee doen
            return new EntityChange<EntityType>(EntityState.Modified, tuple.Parent as EntityType, tuple.Parent as EntityType);
        }
    }
}
