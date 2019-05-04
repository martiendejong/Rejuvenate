using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public interface IDbContextSaveHandler : IDisposable
    {
        void SaveStart(IDbContextWithSaveEvent context);

        void SaveCompleted(IDbContextWithSaveEvent context);
    }
}
