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
    public class DbContextChangesHub
    {
        public List<IEntityChangesHub> Hubs = new List<IEntityChangesHub>();

        public void Subscribe<EntityType>(EntitiesChangedHandler<EntityType> handler) where EntityType : class, new()
        {
            EntityChangesHub<EntityType> hubs = GetHub<EntityType>();
            hubs.Add(handler);
        }

        private EntityChangesHub<EntityType> GetHub<EntityType>() where EntityType : class, new()
        {
            var hub = Hubs.OfType<EntityChangesHub<EntityType>>().FirstOrDefault();
            if (hub == null)
            {
                hub = new EntityChangesHub<EntityType>();
                Hubs.Add(hub);
            }

            return hub;
        }

        public void GatherChanges(DbContext context)
        {
            context.ChangeTracker.DetectChanges();
            var relations = context.GetRelationships();
            Hubs.ForEach(hub => hub.GatherChanges(context.ChangeTracker, relations));
        }

        public void PublishChanges(DbContext context)
        {
            Hubs.ForEach(hub => hub.PublishChanges());
        }
    }
}
