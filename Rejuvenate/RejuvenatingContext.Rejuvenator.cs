using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public abstract partial class RejuvenatingDbContext
    {
        protected class EntityRejuvenator<EntityType> : IEntityRejuvenator where EntityType : class, new()
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
                DbContext.PrepareRejuvenation<EntityType>();
            }

            public void Rejuvenate()
            {
                DbContext.Rejuvenate<EntityType>();
            }
        }
    }
}
