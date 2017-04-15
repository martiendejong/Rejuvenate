using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.Db
{
    public class ChangePublisher<EntityType> : IChangePublisher<EntityType> where EntityType : class
    {
        public int Id { get; set; }

        public Expression<Func<EntityType, bool>> Query { get; set; }

        public EntitiesChangedHandler<EntityType> EntitiesChanged { get; set; }
    }
}
