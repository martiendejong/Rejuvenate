using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2.EntityChangePublishing
{
    public class EntityChangeMessage<EntityType> where EntityType : class, new()
    {
        public EntityChangeMessage(EntityState state, EntityType last, EntityType current)
        {
            State = state;
            Last = last;
            Current = current;
        }

        public EntityState State;
        public EntityType Last;
        public EntityType Current;
    }
}
