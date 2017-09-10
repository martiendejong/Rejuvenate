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

        public void GatherChanges(DbChangeTracker changeTracker)
        {
            var entries = changeTracker.Entries<EntityType>();
            GatheredChanges = entries.Select(GetEntityChangedMessage).ToList().Where(m => m != null);
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
    }
}
