using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IChangeTrackerHandler<EntityType> : IDisposable where EntityType : class, new()
    {
        void Process(IEnumerable<EntityChange<EntityType>> entities);
    }

    public interface IChangeTrackerHandler : IDisposable
    {
        void EntitiesChanged<EntityType>(IEnumerable<EntityChange<EntityType>> entities) where EntityType : class, new();
    }
}
