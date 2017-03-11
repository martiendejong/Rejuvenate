using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public abstract partial class RejuvenatingDbContext : DbContext
    {
        protected class EntityRejuvenator<T> : IEntityRejuvenator where T : class, new()
        {
            public DbChangeTracker ChangeTracker;
            public RejuvenatingDbContext DbContext;

            public EntityRejuvenator(DbChangeTracker changeTracker, RejuvenatingDbContext context)
            {
                ChangeTracker = changeTracker;
                DbContext = context;
            }

            public void PrepareRejuvenation()
            {
                DbContext.PrepareRejuvenation<T>();
            }

            public void Rejuvenate()
            {
                DbContext.Rejuvenate<T>();
            }
        }
    }
}
