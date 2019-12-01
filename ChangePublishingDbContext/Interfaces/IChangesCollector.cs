using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace Rejuvenate
{
    public interface IChangesCollector
    {
        void CollectChanges(DbChangeTracker changeTracker, IEnumerable<EntityRelationChange> relations);

        void PublishChanges();
    }
}
