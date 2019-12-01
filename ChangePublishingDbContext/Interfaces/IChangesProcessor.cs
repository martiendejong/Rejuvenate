using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public interface IChangesProcessor<EntityType> where EntityType : class, new()
    {
        void Process(IEnumerable<EntityChange<EntityType>> entities);
    }
}
