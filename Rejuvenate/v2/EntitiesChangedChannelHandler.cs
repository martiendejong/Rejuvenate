using Rejuvenate.v2.EntityChangePublishing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2
{
    public delegate void EntitiesChangedHandler<EntityType>(IEnumerable<EntityChangeMessage<EntityType>> entities, EntitiesChangedListener<EntityType> listener) where EntityType : class, new();
}
