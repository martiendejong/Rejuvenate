using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public delegate void ValueChangedHandler<ValueType>(ValueType value);

    public delegate void EntitiesChangedHandler<EntityType>(IEnumerable<EntityChange<EntityType>> entities) where EntityType : class, new();

    public interface IChangesPublisher<EntityType> where EntityType : class, new()
    {
        event EntitiesChangedHandler<EntityType> EntitiesChanged;
    }
}
