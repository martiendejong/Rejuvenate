using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IChangePublishingDbContext : IDbContextWithCustomDbSet, IDbContextWithSaveEvent
    {
        IEntityChangeTracker EntityChangeTracker { get; }
    }
}
