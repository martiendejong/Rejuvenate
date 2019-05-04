using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ChangePublishingDbContext;
using System.Data.Entity;

namespace ChangePublishingDbContext
{
    public class ChangePublishingQueryable<EntityType> : IChangePublishingQueryable<EntityType> where EntityType : class, new()
    {
        private IConditionalChangeTrackerManager<EntityType> _changeTracker;

        private IQueryable<EntityType> _queryable;

        private Expression<Func<EntityType, bool>> _expression;

        public Expression<Func<EntityType, bool>> Condition { get => _expression; set => _expression = value; }

        public ChangePublishingQueryable(IQueryable<EntityType> queryable, IConditionalChangeTrackerManager<EntityType> changeTracker, Expression<Func<EntityType, bool>> expression)
        {
            _queryable = queryable;
            _changeTracker = changeTracker;
            _expression = expression;
        }

        public IChangePublishingQueryable<EntityType> Where(Expression<Func<EntityType, bool>> expression)
        {
            return new ChangePublishingQueryable<EntityType>(_queryable.Where(expression), _changeTracker, And(expression));
        }

        protected Expression<Func<EntityType, bool>> And(Expression<Func<EntityType, bool>> expression)
        {
            return _expression == null ? expression : _expression.And(expression);
        }

        public event EntitiesChangedHandler<EntityType> EntitiesChanged
        {
            add
            {
                _changeTracker.Where(_expression).EntitiesChanged += value;
            }
            remove
            {
                _changeTracker.Where(_expression).EntitiesChanged -= value;
            }
        }

        #region implement IQueryable

        public Type ElementType => _queryable.ElementType;

        public Expression Expression => _queryable.Expression;

        public IQueryProvider Provider => _queryable.Provider;

        public IEnumerator<EntityType> GetEnumerator() => _queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _queryable.GetEnumerator();

        #endregion
    }
}
