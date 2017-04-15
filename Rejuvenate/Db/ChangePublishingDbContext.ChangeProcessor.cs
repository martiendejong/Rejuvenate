using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.Db
{
    public abstract partial class ChangePublishingDbContext
    {
        protected class ChangeProcessor<EntityType> : IChangeProcessor where EntityType : class, new()
        {
            public ChangePublishingDbContext DbContext;

            public ChangeProcessor(ChangePublishingDbContext context)
            {
                DbContext = context;
            }

            public void ProcessChanges()
            {
                DbContext.GatherChanges<EntityType>();
            }

            public void Publish()
            {
                DbContext.Publish<EntityType>();
            }
        }
    }
}
