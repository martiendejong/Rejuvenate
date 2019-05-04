using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IEntityChangeTracker : IDbContextSaveHandler
    {
        IChangeTracker<EntityType> Entity<EntityType>() where EntityType : class, new();
    }
}
