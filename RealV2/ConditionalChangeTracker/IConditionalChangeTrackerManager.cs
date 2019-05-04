using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IConditionalChangeTrackerManager<EntityType> where EntityType : class, new()
    {
        IConditionalChangeTracker<EntityType> Where(Expression<Func<EntityType, bool>> expression);
    }
}
