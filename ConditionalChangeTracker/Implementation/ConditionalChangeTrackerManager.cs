using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public class ConditionalChangeTrackerManager<EntityType> : AChangeTrackerHandler<EntityType>, IConditionalChangeTrackerManager<EntityType> where EntityType : class, new()
    {
        protected List<IConditionalChangeTracker<EntityType>> ConditionalChangeTrackers = new List<IConditionalChangeTracker<EntityType>>();

        public ConditionalChangeTrackerManager(IChangeTracker<EntityType> changeTracker) : base(changeTracker)
        {
        }

        public override void EntitiesChanged(IEnumerable<EntityChange<EntityType>> entities)
        {
            ConditionalChangeTrackers.ForEach(tracker => tracker.Process(entities));
        }

        public IConditionalChangeTracker<EntityType> Where(Expression<Func<EntityType, bool>> expression)
        {
            var tracker = ConditionalChangeTrackers.FirstOrDefault(t => LambdaCompare.Eq(t.Expression, expression));
            if (tracker == null)
            {
                tracker = new ConditionalChangeTracker<EntityType>(expression);
                ConditionalChangeTrackers.Add(tracker);
            }
            return tracker;
        }
    }
}
