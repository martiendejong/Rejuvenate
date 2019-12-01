using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public interface IChangesPublisherFactory : IDbContextSaveHandler
    {
        IChangesPublisher<EntityType> Entity<EntityType>() where EntityType : class, new();
    }
}
