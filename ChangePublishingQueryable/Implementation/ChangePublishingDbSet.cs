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
    public class ChangePublishingDbSet<EntityType> : AChangePublishingQueryable<EntityType>, IChangePublishingDbSet<EntityType> where EntityType : class, new()
    {
        public ChangePublishingDbSet(DbContext db, IDbSet<EntityType> dbSet, IChangeTracker<EntityType> changeTracker)
            : base(db, dbSet, new ConditionalChangeTrackerFactory<EntityType>(changeTracker), new MapChangeTrackerFactory<EntityType>(changeTracker, db))
        {
            _dbSet = dbSet;
            _changeTracker = changeTracker;
        }

        private IChangeTracker<EntityType> _changeTracker;

        private IDbSet<EntityType> _dbSet;

        public override Expression<Func<EntityType, bool>> Filter => (x) => true;

        protected override Expression<Func<EntityType, bool>> CombineFilter(Expression<Func<EntityType, bool>> expression) => expression;

        public override event EntitiesChangedHandler<EntityType> EntitiesChanged
        {
            add
            {
                _changeTracker.EntitiesChanged += value;
            }
            remove
            {
                _changeTracker.EntitiesChanged -= value;
            }
        }

        #region implement IDbSet<EntityType>

        public ObservableCollection<EntityType> Local => _dbSet.Local;

        public EntityType Add(EntityType entity)
        {
            return _dbSet.Add(entity);
        }

        public EntityType Attach(EntityType entity)
        {
            return _dbSet.Attach(entity);
        }

        public EntityType Create()
        {
            return _dbSet.Create();
        }

        public EntityType Find(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public EntityType Remove(EntityType entity)
        {
            return _dbSet.Remove(entity);
        }

        TDerivedEntity IDbSet<EntityType>.Create<TDerivedEntity>()
        {
            return _dbSet.Create<TDerivedEntity>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dbSet.GetEnumerator();
        }

        #endregion
    }
}
