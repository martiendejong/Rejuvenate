using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public abstract class AChangeTrackerHandler<EntityType> : IChangeTrackerHandler<EntityType>, IDisposable where EntityType : class, new()
    {
        protected IChangeTracker<EntityType> ChangeTracker;

        public AChangeTrackerHandler(IChangeTracker<EntityType> changeTracker)
        {
            ChangeTracker = changeTracker;
            changeTracker.EntitiesChanged += Process;
        }

        abstract public void Process(IEnumerable<EntityChange<EntityType>> entities);

        public void Dispose()
        {
            ChangeTracker.EntitiesChanged -= Process;
        }
    }
}
