using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2.EntityChangePublishing
{
    public interface IEntityChangesHub
    {
        void GatherChanges(DbChangeTracker changeTracker, IEnumerable<Tuple<object, object, EntityState>> relations);

        void PublishChanges();

        void Add(object handler);
    }
}
