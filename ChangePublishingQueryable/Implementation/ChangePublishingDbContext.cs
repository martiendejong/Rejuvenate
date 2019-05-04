using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ChangePublishingDbContext.Implementation;
using ChangePublishingDbContext;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
using System.Data.Entity.Core.Objects;

namespace ChangePublishingDbContext
{
    public class ChangePublishingDbContext : DbContextWithCustomDbSet, IChangePublishingDbContext
    {
        #region ChangeTracker property

        private IEntityChangeTracker _entityChangeTracker;

        public IEntityChangeTracker EntityChangeTracker => _entityChangeTracker == null ? _entityChangeTracker = new EntityChangeTracker(this) : _entityChangeTracker;

        #endregion

        #region Implement abstract DbContextWithCustomDbSet

        protected override string _derivedSetName => "IChangePublishingDbSet";

        public override IDbSet<EntityType> GetDerivedSet<EntityType>(DbSet<EntityType> dbSet)
        {
            return new ChangePublishingDbSet<EntityType>(dbSet, EntityChangeTracker.Entity<EntityType>());
        }

        #endregion

        #region Implement IDbContextWithSaveEvent

        public event DbContextEventHandler SaveStart;

        public event DbContextEventHandler SaveCompleted;

        public override Task<int> SaveChangesAsync()
        {
            SaveStart?.Invoke(this);
            var task = base.SaveChangesAsync();
            task.GetAwaiter().OnCompleted(() => SaveCompleted?.Invoke(this));
            return task;
        }

        public override int SaveChanges()
        {
            SaveStart?.Invoke(this);
            var res = base.SaveChanges();
            SaveCompleted?.Invoke(this);
            return res;
        }

        #endregion

        #region Inherit constructors from the base class

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext() : base() { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(DbCompiledModel model) : base(model) { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(string nameOrconnectionString) : base(nameOrconnectionString) { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(string nameOrConnectiongString, DbCompiledModel model) : base(nameOrConnectiongString, model) { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(ObjectContext objectContext, bool contextOwnsObjectContext) : base(objectContext, contextOwnsObjectContext) { }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { }

        #endregion
    }
}
