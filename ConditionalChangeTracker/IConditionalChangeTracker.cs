using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IConditionalChangeTracker<EntityType> : IChangeTracker<EntityType> where EntityType : class, new()
    {
        Expression<Func<EntityType, bool>> Expression { get; }

        void Process(IEnumerable<EntityChange<EntityType>> entities);
    }
}
