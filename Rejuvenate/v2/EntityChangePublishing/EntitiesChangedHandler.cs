using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2.EntityChangePublishing
{
    public delegate void EntitiesChangedHandler<EntityType>(IEnumerable<EntityChangeMessage<EntityType>> entities) where EntityType : class, new();
}
