using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public delegate void RejuvenateClientCallback<T>(Type type, int rejuvenatorId, EntityState state, IEnumerable<T> entities) where T : class;

    public class ClientRejuvenator<T> : IClientRejuvenator<T> where T : class
    {
        public int Id { get; set; }

        public Expression<Func<T, bool>> Expression { get; set; }

        public RejuvenateClientCallback<T> Rejuvenate { get; set; }
    }
}
