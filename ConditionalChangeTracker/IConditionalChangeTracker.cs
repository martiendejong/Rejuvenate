using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IEntityChangeProcessor<EntityType> where EntityType : class, new()
    {
        void Process(IEnumerable<EntityChange<EntityType>> entities);
    }

    public interface IConditionalChangeTracker<EntityType> : IEntityChangeProcessor<EntityType>, IChangeTracker<EntityType> where EntityType : class, new()
    {
        Expression<Func<EntityType, bool>> Expression { get; }
    }

    public interface IMapChangeTracker<EntityType, ToEntityType> : IEntityChangeProcessor<EntityType>, IChangeTracker<ToEntityType> where ToEntityType : class, new() where EntityType : class, new()
    {
        Expression<Func<EntityType, ToEntityType>> Mapping { get; }
    }

}
