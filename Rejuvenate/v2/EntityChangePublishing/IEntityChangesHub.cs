using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2.EntityChangePublishing
{
    public interface IEntityChangesHub
    {
        void GatherChanges(DbChangeTracker changeTracker);

        void PublishChanges();

        void Add(object handler);
    }
}
