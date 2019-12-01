using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public interface IEntityChangeFilterProcessorFactory<EntityType> where EntityType : class, new()
    {
        IChangesFilterProcessor<EntityType> Where(Expression<Func<EntityType, bool>> filter);
    }
}
