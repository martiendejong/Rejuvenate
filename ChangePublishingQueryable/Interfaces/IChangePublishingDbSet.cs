using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public interface IChangePublishingDbSet<EntityType> : IDbSet<EntityType>, IChangePublishingQueryable<EntityType>, IEnumerable<EntityType>, IQueryable, IEnumerable where EntityType : class, new()
    {
    }
}
