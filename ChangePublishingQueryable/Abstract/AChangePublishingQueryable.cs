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
    public abstract class AChangePublishingQueryable<EntityType> : IChangePublishingQueryable<EntityType> where EntityType : class, new()
    {
        protected IEntityChangeFilterProcessorFactory<EntityType> _filterProcessorFactory;

        protected IEntityChangeMappingProcessorFactory<EntityType> _mappingProcessorFactory;

        public abstract Expression<Func<EntityType, bool>> Filter { get; }

        public abstract event EntitiesChangedHandler<EntityType> EntitiesChanged;

        protected IQueryable<EntityType> _queryable;

        protected DbContext _db;

        public AChangePublishingQueryable(DbContext db, IQueryable<EntityType> queryable, IEntityChangeFilterProcessorFactory<EntityType> filterProcessorFactory, IEntityChangeMappingProcessorFactory<EntityType> mappingProcessorFactory)
        {
            _db = db;
            _queryable = queryable;
            _filterProcessorFactory = filterProcessorFactory;
            _mappingProcessorFactory = mappingProcessorFactory;
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
            return new ChangePublishingQueryable<EntityType>(_db, _queryable.Where(filter), _filterProcessorFactory, _mappingProcessorFactory, CombineFilter(filter));
        }

        public IChangePublishingQueryable<ToEntityType> Select<ToEntityType>(Expression<Func<EntityType, ToEntityType>> mapping) where ToEntityType : class, new()
        {
            var mappingProcessor = _mappingProcessorFactory.Select(mapping);
            var filterProcessorFactory = new EntityChangeFilterProcessorFactory<ToEntityType>(mappingProcessor);
            var mappingProcessorFactory = new EntityChangeMappingProcessorFactory<ToEntityType>(mappingProcessor, _db);
            return new ChangePublishingQueryable<ToEntityType>(_db, _queryable.Select(mapping), filterProcessorFactory, mappingProcessorFactory, null);
        }


        #region implement group functions

        public IChangePublishingValue<EntityType, int> Sum(Expression<Func<EntityType, int>> selector)
        {
            return ExecGroupFunction(queryable => queryable.Sum(selector));
        }

        public IChangePublishingValue<EntityType, double> Sum(Expression<Func<EntityType, double>> selector)
        {
            return ExecGroupFunction(queryable => queryable.Sum(selector));
        }

        public IChangePublishingValue<EntityType, double> Average(Expression<Func<EntityType, double>> selector)
        {
            return ExecGroupFunction(queryable => queryable.Average(selector));
        }

        public IChangePublishingValue<EntityType, double> Max(Expression<Func<EntityType, double>> selector)
        {
            return ExecGroupFunction(queryable => queryable.Max(selector));
        }

        public IChangePublishingValue<EntityType, int> Max(Expression<Func<EntityType, int>> selector)
        {
            return ExecGroupFunction(queryable => queryable.Max(selector));
        }
        public IChangePublishingValue<EntityType, int> Min(Expression<Func<EntityType, int>> selector)
        {
            return ExecGroupFunction(queryable => queryable.Min(selector));
        }

        public IChangePublishingValue<EntityType, double> Min(Expression<Func<EntityType, double>> selector)
        {
            return ExecGroupFunction(queryable => queryable.Min(selector));
        }

        public IChangePublishingValue<EntityType, ValueType> ExecGroupFunction<ValueType>(Expression<Func<IQueryable<EntityType>, ValueType>> expression)
        {
            return new ChangePublishingValue<EntityType, ValueType>(this, expression);
        }

        #endregion

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
