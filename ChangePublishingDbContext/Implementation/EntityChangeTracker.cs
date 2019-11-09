using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext.Implementation
{
    public class EntityChangeTracker : ADbContextSaveHandler, IEntityChangeTracker
    {
        protected List<IChangesCollector> ChangeCollectors = new List<IChangesCollector>();

        public EntityChangeTracker(IDbContextWithSaveEvent context) : base(context)
        {
        }

        override public void SaveStart(IDbContextWithSaveEvent context)
        {
            context.ChangeTracker.DetectChanges();
            var relations = context.GetChangedRelationships();
            ChangeCollectors.ForEach(collector => collector.CollectChanges(context.ChangeTracker, relations));
        }

        override public void SaveCompleted(IDbContextWithSaveEvent context)
        {
            ChangeCollectors.ForEach(collector => collector.PublishChanges());
        }

        public IChangeTracker<EntityType> Entity<EntityType>() where EntityType : class, new()
        {
            var collector = ChangeCollectors.OfType<IChangeTracker<EntityType>>().FirstOrDefault();
            if(collector == null)
            {
                var newCollector = new ChangeTracker<EntityType>();
                ChangeCollectors.Add(newCollector);
                collector = newCollector;
            }
            return collector;
        }
    }
}
