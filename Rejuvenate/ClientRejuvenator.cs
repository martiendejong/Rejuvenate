using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public delegate void RejuvenateClientCallback<EntityType>(Type type, int rejuvenatorId, EntityState state, IEnumerable<EntityType> entities) where EntityType : class;

    public class ClientRejuvenator<EntityType> : IClientRejuvenator<EntityType> where EntityType : class
    {
        public int Id { get; set; }

        public Expression<Func<EntityType, bool>> Expression { get; set; }

        public RejuvenateClientCallback<EntityType> Rejuvenate { get; set; }
    }
}
