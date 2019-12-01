using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Rejuvenate;
using System.Data.Entity;
using Microsoft.AspNet.SignalR.Hubs;

namespace Rejuvenate
{
    public class ChangePublishingQueryable<EntityType> : AChangePublishingQueryable<EntityType> where EntityType : class, new()
    {
        public ChangePublishingQueryable(DbContext db, IQueryable<EntityType> queryable, IEntityChangeFilterProcessorFactory<EntityType> changeTrackerFactory, IEntityChangeMappingProcessorFactory<EntityType> mapChangeTrackerFactory, Expression<Func<EntityType, bool>> expression)
            : base(db, queryable, changeTrackerFactory, mapChangeTrackerFactory)
        {
            _filter = expression;
        }

        private Expression<Func<EntityType, bool>> _filter;

        public override Expression<Func<EntityType, bool>> Filter { get => _filter; }

        override public event EntitiesChangedHandler<EntityType> EntitiesChanged
        {
            add
            {
                _filterProcessorFactory.Where(_filter).EntitiesChanged += value;
            }
            remove
            {
                _filterProcessorFactory.Where(_filter).EntitiesChanged -= value;
            }
        }

        protected override Expression<Func<EntityType, bool>> CombineFilter(Expression<Func<EntityType, bool>> filter) => And(filter);

        protected Expression<Func<EntityType, bool>> And(Expression<Func<EntityType, bool>> filter)
        {
            return Filter == null ? filter : Filter.And(filter);
        }
    }
}
