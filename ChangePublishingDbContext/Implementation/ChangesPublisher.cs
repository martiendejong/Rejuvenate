using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.Implementation
{
    public class ChangesPublisher<EntityType> : IChangesCollector, IChangesPublisher<EntityType> where EntityType : class, new()
    {
        public event EntitiesChangedHandler<EntityType> EntitiesChanged;

        public void CollectChanges(DbChangeTracker changeTracker, IEnumerable<EntityRelationChange> relations)
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

        #region entity field changes

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

        #endregion

        #region entity relation changes

        protected static EntityChange<EntityType> GetRelationshipChangedMessage(EntityRelationChange changedRelationship)
        {
            switch (changedRelationship.State)
            {
                case EntityState.Added:
                    return GetRelationshipAddedMessage(changedRelationship);
                case EntityState.Deleted:
                    return GetRelationshipDeletedMessage(changedRelationship);
                default:
                    throw new Exception("Only added or deleted relationships are supported");
            }
        }

        protected static EntityChange<EntityType> GetRelationshipAddedMessage(EntityRelationChange changedRelationship)
        {
            return new EntityChange<EntityType>(EntityState.Modified, changedRelationship.Parent as EntityType, changedRelationship.Parent as EntityType);
        }

        protected static EntityChange<EntityType> GetRelationshipDeletedMessage(EntityRelationChange changedRelationship)
        {//hier nog iets mee doen
            return new EntityChange<EntityType>(EntityState.Modified, changedRelationship.Parent as EntityType, changedRelationship.Parent as EntityType);
        }

        #endregion
    }
}
