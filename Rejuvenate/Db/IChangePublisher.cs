using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.Db
{
    public interface IChangePublisher<EntityType> where EntityType : class
    {
        int Id { get; set; }

        Expression<Func<EntityType, bool>> Query { get; set; }

        EntitiesChangedHandler<EntityType> EntitiesChanged { get; set; }
    }
}
