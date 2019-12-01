using System;
using System.Linq.Expressions;

namespace Rejuvenate
{
    public interface IEntityChangeMappingProcessorFactory<EntityType> where EntityType : class, new()
    {
        IEntityChangeMappingProcessor<EntityType, ToEntityType> Select<ToEntityType>(Expression<Func<EntityType, ToEntityType>> mapping) where ToEntityType : class, new();
    }
}
