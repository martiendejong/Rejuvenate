using System;
using System.Linq.Expressions;

namespace Rejuvenate
{
    public interface IEntityChangeMappingProcessor<EntityType, ToEntityType> : IChangesProcessor<EntityType>, IChangesPublisher<ToEntityType> where ToEntityType : class, new() where EntityType : class, new()
    {
        Expression<Func<EntityType, ToEntityType>> Mapping { get; }
    }

}
