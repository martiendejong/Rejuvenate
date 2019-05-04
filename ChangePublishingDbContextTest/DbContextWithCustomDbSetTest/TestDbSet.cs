using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;

namespace ChangePublishingDbContextTest
{
    public class TestDbSet<EntityType> : IDbSet<EntityType> where EntityType : class
    {
        public TestDbSet(IDbSet<EntityType> dbSet)
        {
            _dbSet = dbSet;
        }

        private IDbSet<EntityType> _dbSet;

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

        #endregion
    }
}
