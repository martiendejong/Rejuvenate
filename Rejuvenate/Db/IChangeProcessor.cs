using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.Db
{
    public interface IChangeProcessor
    {
        void ProcessChanges();

        void Publish();
    }
}
