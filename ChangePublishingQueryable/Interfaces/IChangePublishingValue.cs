using System;
using System.Linq;
using System.Linq.Expressions;

namespace Rejuvenate
{
    public interface IChangePublishingValue<EntityType, ValueType> where EntityType : class, new()
    {
        event ValueChangedHandler<ValueType> ValueChanged;

        IChangePublishingQueryable<EntityType> Queryable { get; }

        Expression<Func<IQueryable<EntityType>, ValueType>> Expression { get; }

        ValueType Value { get; }
    }
}