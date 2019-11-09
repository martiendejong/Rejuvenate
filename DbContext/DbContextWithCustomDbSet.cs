using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public abstract class ADbContextWithCustomDbSet : DbContext, IDbContextWithCustomDbSet
    {
        abstract protected string _customDbSetClassName { get; }

        abstract public IDbSet<EntityType> GetCustomDbSet<EntityType>(DbSet<EntityType> dbSet) where EntityType : class, new();

        private Dictionary<Type, object> _dbSets;

        new public IDbSet<EntityType> Set<EntityType>() where EntityType : class, new()
        {
            //Instantiate _dbSets if required
            if (_dbSets == null)
            {
                _dbSets = new Dictionary<Type, object>();
            }

            //If already resolved, return stored reference
            if (_dbSets.ContainsKey(typeof(EntityType)))
            {
                return (IDbSet<EntityType>)_dbSets[typeof(EntityType)];
            }

            //Otherwise resolve, store reference and return 
            var resolvedSet = GetCustomDbSet(base.Set<EntityType>());
            _dbSets.Add(typeof(EntityType), resolvedSet);
            return resolvedSet;
        }

        private void AssignDerivedSets()
        {
            GetDerivedSetsAndTypes(_customDbSetClassName).ToList().ForEach(derivedSet => derivedSet.Key.SetValue(this, GetGenericSetMethod(derivedSet.Value).Invoke(this, null)));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            GetDerivedSetsAndTypes(_customDbSetClassName).ToList().ForEach(derivedSet => GetGenericModelBuilderEntityMethod(modelBuilder, derivedSet.Value).Invoke(modelBuilder, null));
        }

        #region reflection stuff

        protected MethodInfo GetGenericSetMethod(Type entityType)
        {
            var genericSet = GetType().GetMethods().FirstOrDefault(m =>
                   m.IsGenericMethod &&
                   m.Name.StartsWith("Set") &&
                   m.GetGenericArguments().Count() == 1);
            return genericSet.MakeGenericMethod(entityType);
        }

        protected MethodInfo GetGenericModelBuilderEntityMethod(DbModelBuilder modelBuilder, Type entityType)
        {
            var genericEntityMethod = modelBuilder.GetType().GetMethods().FirstOrDefault(m =>
                        m.IsGenericMethod &&
                        m.Name.StartsWith("Entity") &&
                        m.GetGenericArguments().Count() == 1);
            return genericEntityMethod.MakeGenericMethod(entityType);
        }

        protected List<PropertyInfo> GetDerivedSets(string derivedSetName)
        {
            return GetType().GetProperties().Where(p =>
                    p.PropertyType.IsInterface &&
                    p.PropertyType.IsGenericType &&
                    p.PropertyType.Name.StartsWith(derivedSetName) &&
                    p.PropertyType.GetGenericArguments().Count() == 1)
                    .ToList();
        }

        protected Dictionary<PropertyInfo, Type> GetDerivedSetsAndTypes(string derivedSetName)
        {
            return GetDerivedSets(derivedSetName).ToDictionary(
                    derivedSet => derivedSet,
                    derivedSet => derivedSet.PropertyType.GetGenericArguments().FirstOrDefault()
                )
                .Where(pair => pair.Value != null)
                .ToDictionary(p => p.Key, p => p.Value);
        }

        #endregion

        #region Inherit constructors from the base class

        ///<summary>Same as with DbContext</summary>
        public ADbContextWithCustomDbSet() : base() { AssignDerivedSets(); }

        ///<summary>Same as with DbContext</summary>
        public ADbContextWithCustomDbSet(DbCompiledModel model) : base(model) { AssignDerivedSets(); }

        ///<summary>Same as with DbContext</summary>
        public ADbContextWithCustomDbSet(string nameOrconnectionString) : base(nameOrconnectionString) { AssignDerivedSets(); }

        ///<summary>Same as with DbContext</summary>
        public ADbContextWithCustomDbSet(string nameOrConnectiongString, DbCompiledModel model) : base(nameOrConnectiongString, model) { AssignDerivedSets(); }

        ///<summary>Same as with DbContext</summary>
        public ADbContextWithCustomDbSet(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { AssignDerivedSets(); }

        ///<summary>Same as with DbContext</summary>
        public ADbContextWithCustomDbSet(ObjectContext objectContext, bool contextOwnsObjectContext) : base(objectContext, contextOwnsObjectContext) { AssignDerivedSets(); }

        ///<summary>Same as with DbContext</summary>
        public ADbContextWithCustomDbSet(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { AssignDerivedSets(); }

        #endregion
    }
}
