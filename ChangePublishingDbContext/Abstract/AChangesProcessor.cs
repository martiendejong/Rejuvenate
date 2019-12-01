using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public abstract class AChangesProcessor<EntityType> : IChangesProcessor<EntityType>, IDisposable where EntityType : class, new()
    {
        protected IChangesPublisher<EntityType> Bus;

        public AChangesProcessor(IChangesPublisher<EntityType> bus)
        {
            Bus = bus;
            bus.EntitiesChanged += Process;
        }

        abstract public void Process(IEnumerable<EntityChange<EntityType>> entities);

        public void Dispose()
        {
            Bus.EntitiesChanged -= Process;
        }
    }
}
