using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext.Implementation
{
    public class EntityChangeTracker : ADbContextSaveHandler, IEntityChangeTracker
    {
        public List<IChangesProcessor> ChangeTrackers = new List<IChangesProcessor>();

        public EntityChangeTracker(IDbContextWithSaveEvent context) : base(context)
        {
        }

        override public void SaveStart(IDbContextWithSaveEvent context)
        {
            context.ChangeTracker.DetectChanges();
            var relations = context.GetChangedRelationships();
            ChangeTrackers.ForEach(collector => collector.GatherChanges(context.ChangeTracker, relations));
        }

        override public void SaveCompleted(IDbContextWithSaveEvent context)
        {
            ChangeTrackers.ForEach(collector => collector.PublishChanges());
        }

        public IChangeTracker<EntityType> Entity<EntityType>() where EntityType : class, new()
        {
            var collector = ChangeTrackers.OfType<IChangeTracker<EntityType>>().FirstOrDefault();
            if(collector == null)
            {
                var newCollector = new ChangeTracker<EntityType>();
                ChangeTrackers.Add(newCollector);

                collector = newCollector;
            }
            return collector;
        }
    }
}
