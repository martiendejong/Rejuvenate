using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.Db
{
    public delegate void EntitiesChangedHandler<EntityType>(Type type, int rejuvenatorId, EntityState state, IEnumerable<KeyValuePair<EntityType, EntityType>> entities) where EntityType : class;
}
