using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.Implementation
{
    public class ChangesPublisherFactory : ADbContextSaveHandler, IChangesPublisherFactory
    {
        protected List<IChangesCollector> ChangeCollectors = new List<IChangesCollector>();

        public ChangesPublisherFactory(IDbContextWithSaveEvent context) : base(context)
        {
        }

        #region implement ADbContextSaveHandler, routes event to the stored ChangeCollectors

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

        #endregion

        #region implement IChangesPublisherFactory

        public IChangesPublisher<EntityType> Entity<EntityType>() where EntityType : class, new()
        {
            var collector = ChangeCollectors.OfType<IChangesPublisher<EntityType>>().FirstOrDefault();
            if(collector == null)
            {
                var newCollector = new ChangesPublisher<EntityType>();
                ChangeCollectors.Add(newCollector);
                collector = newCollector;
            }
            return collector;
        }

        #endregion
    }
}
