using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public class ChangePublishingValue<EntityType, ValueType> : IChangePublishingValue<EntityType, ValueType> where EntityType : class, new()
    {
        // todo goed doen
        public event ValueChangedHandler<ValueType> ValueChanged
        {
            add
            {
                Queryable.EntitiesChanged += (e) =>
                {
                    Value = Func.Invoke(Queryable);
                    value(Value);
                };
            }
            remove
            {
                // todo
                //_entityChangeFilterProcessorFactory.Where(_filter).EntitiesChanged -= value;
            }
        }

        private Func<IQueryable<EntityType>, ValueType> Func;

        public IChangePublishingQueryable<EntityType> Queryable { get; }

        public Expression<Func<IQueryable<EntityType>, ValueType>> Expression { get; }

        public ValueType Value { get; private set; }

        public ChangePublishingValue(IChangePublishingQueryable<EntityType> queryable, Expression<Func<IQueryable<EntityType>, ValueType>> expression)
        {
            Queryable = queryable;
            Expression = expression;
            Func = Expression.Compile();
            Value = Func.Invoke(Queryable);
        }
    }
}
