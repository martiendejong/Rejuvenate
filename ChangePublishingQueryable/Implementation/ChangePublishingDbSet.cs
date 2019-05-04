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
    public class ChangePublishingDbSet<EntityType> : IChangePublishingDbSet<EntityType> where EntityType : class, new()
    {
        private IChangeTracker<EntityType> _changeTracker;

        public ChangePublishingDbSet(IDbSet<EntityType> dbSet, IChangeTracker<EntityType> changeTracker)
        {
            _dbSet = dbSet;
            _changeTracker = changeTracker;
        }

        private IDbSet<EntityType> _dbSet;

        public Expression<Func<EntityType, bool>> Condition { get => (x) => true; set { } }

        public event EntitiesChangedHandler<EntityType> EntitiesChanged
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

        public ChangePublishingQueryable<EntityType> Where(Expression<Func<EntityType, bool>> expression)
        {
            var condtionalChangeTracker = new ConditionalChangeTrackerManager<EntityType>(_changeTracker);
            return new ChangePublishingQueryable<EntityType>(_dbSet.Where(expression), condtionalChangeTracker, expression);
        }

        #region implement IDbSet<EntityType>

        public Type ElementType => _dbSet.ElementType;

        public Expression Expression => _dbSet.Expression;

        public ObservableCollection<EntityType> Local => _dbSet.Local;

        public IQueryProvider Provider => _dbSet.Provider;

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

        public IEnumerator<EntityType> GetEnumerator()
        {
            return _dbSet.GetEnumerator();
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

        IChangePublishingQueryable<EntityType> IChangePublishingQueryable<EntityType>.Where(Expression<Func<EntityType, bool>> expression)
        {
            return new ChangePublishingQueryable<EntityType>(_dbSet.Where(expression), new ConditionalChangeTrackerManager<EntityType>(_changeTracker), expression);
        }

        #endregion
    }
}
