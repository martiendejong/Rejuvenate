using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public abstract class ADbContextSaveHandler : IDbContextSaveHandler, IDisposable
    {
        public ADbContextSaveHandler(IDbContextWithSaveEvent context)
        {
            Context = context;
            context.SaveStart += SaveStart;
            context.SaveCompleted += SaveCompleted;
        }

        abstract public void SaveStart(IDbContextWithSaveEvent context);

        abstract public void SaveCompleted(IDbContextWithSaveEvent context);

        protected IDbContextWithSaveEvent Context;

        public void Dispose()
        {
            Context.SaveStart -= SaveStart;
            Context.SaveCompleted -= SaveCompleted;
        }
    }
}
