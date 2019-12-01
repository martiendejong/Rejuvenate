using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public class EntityChange<EntityType> where EntityType : class, new()
    {
        public EntityChange(EntityState state, EntityType previous, EntityType current)
        {
            State = state;
            Previous = previous;
            Current = current;
        }

        public EntityState State;
        public EntityType Previous;
        public EntityType Current;
    }
}
