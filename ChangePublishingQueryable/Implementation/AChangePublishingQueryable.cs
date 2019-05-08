using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ChangePublishingDbContext;
using System.Data.Entity;
using Microsoft.AspNet.SignalR.Hubs;

namespace ChangePublishingDbContext
{
    public abstract class AChangePublishingQueryable<EntityType> : IChangePublishingQueryable<EntityType> where EntityType : class, new()
    {
        protected IConditionalChangeTrackerFactory<EntityType> _conditionalChangeTrackerFactory;

        protected IMapChangeTrackerFactory<EntityType> _mapChangeTrackerFactory;

        public abstract Expression<Func<EntityType, bool>> Filter { get; }

        public abstract event EntitiesChangedHandler<EntityType> EntitiesChanged;

        protected IQueryable<EntityType> _queryable;

        protected DbContext _db;

        public AChangePublishingQueryable(DbContext db, IQueryable<EntityType> queryable, IConditionalChangeTrackerFactory<EntityType> conditionalChangeTrackerFactory, IMapChangeTrackerFactory<EntityType> mapChangeTrackerFactory)
        {
            _db = db;
            _queryable = queryable;
            _conditionalChangeTrackerFactory = conditionalChangeTrackerFactory;
            _mapChangeTrackerFactory = mapChangeTrackerFactory;
        }

        public IPublisher<EntityType, HubType> Publisher<HubType>() where HubType : IHub
        {
            var publisher = ChangePublishingSignalRHub.Publishers
                .Select(p => p.Value)
                .OfType<Publisher<EntityType, HubType>>()
                .FirstOrDefault(p => LambdaCompare.Eq(p.Condition, Filter));
            if (publisher == null)
            {
                publisher = new Publisher<EntityType, HubType> { Condition = Filter };
                ChangePublishingSignalRHub.Publishers.Add(publisher.Id, publisher);
                EntitiesChanged += publisher.Publish;
            }
            return publisher;
        }

        public IChangePublishingQueryable<EntityType> Where(Expression<Func<EntityType, bool>> filter)
        {
            return new ChangePublishingQueryable<EntityType>(_db, _queryable.Where(filter), _conditionalChangeTrackerFactory, _mapChangeTrackerFactory, CombineFilter(filter));
        }

        public IChangePublishingQueryable<ToEntityType> Select<ToEntityType>(Expression<Func<EntityType, ToEntityType>> mapping) where ToEntityType : class, new()
        {
            var tracker = _mapChangeTrackerFactory.Select(mapping);
            //var tracker = new MapChangeTracker<EntityType, ToEntityType>(_changeTracker, mapping, _db);
            var conditionalChangeTrackerFactory = new ConditionalChangeTrackerFactory<ToEntityType>(tracker);
            var mapChangeTrackerFactory = new MapChangeTrackerFactory<ToEntityType>(tracker, _db);
            return new ChangePublishingQueryable<ToEntityType>(_db, _queryable.Select(mapping), conditionalChangeTrackerFactory, mapChangeTrackerFactory, null);
        }

        protected abstract Expression<Func<EntityType, bool>> CombineFilter(Expression<Func<EntityType, bool>> expression);

        #region implement IQueryable

        public Type ElementType => _queryable.ElementType;

        public Expression Expression => _queryable.Expression;

        public IQueryProvider Provider => _queryable.Provider;

        public IEnumerator<EntityType> GetEnumerator() => _queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _queryable.GetEnumerator();

        #endregion
    }
}
