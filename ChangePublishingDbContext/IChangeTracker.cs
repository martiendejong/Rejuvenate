using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public delegate void EntitiesChangedHandler<EntityType>(IEnumerable<EntityChange<EntityType>> entities) where EntityType : class, new();

    public interface IChangesCollector
    {
        void CollectChanges(DbChangeTracker changeTracker, IEnumerable<ChangedRelationship> relations);

        void PublishChanges();
    }

    public interface IChangeTracker<EntityType> where EntityType : class, new()
    {
        event EntitiesChangedHandler<EntityType> EntitiesChanged;
    }
}
