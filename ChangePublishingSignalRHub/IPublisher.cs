using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public interface IPublisher<EntityType, HubType> : IPublisher where EntityType : class, new()
    {
        Expression<Func<EntityType, bool>> Condition { get; set; }

        void Publish(IEnumerable<EntityChange<EntityType>> changes);
    }

    public interface IPublisher
    {
        Guid Id { get; }

        List<string> ClientIds { get; }
    }
}