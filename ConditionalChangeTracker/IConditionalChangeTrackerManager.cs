using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IConditionalChangeTrackerFactory<EntityType> where EntityType : class, new()
    {
        IConditionalChangeTracker<EntityType> Where(Expression<Func<EntityType, bool>> expression);
    }

    public interface IMapChangeTrackerFactory<EntityType> where EntityType : class, new()
    {
        IMapChangeTracker<EntityType, ToEntityType> Select<ToEntityType>(Expression<Func<EntityType, ToEntityType>> expression) where ToEntityType : class, new();
    }
}
