using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{

    public interface IChangesFilterProcessor<EntityType> : IChangesProcessor<EntityType>, IChangesPublisher<EntityType> where EntityType : class, new()
    {
        Expression<Func<EntityType, bool>> FilterExpression { get; }
    }

}
