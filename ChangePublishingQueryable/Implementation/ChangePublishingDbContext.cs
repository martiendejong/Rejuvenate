using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Rejuvenate.Implementation;
using Rejuvenate;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
using System.Data.Entity.Core.Objects;

namespace Rejuvenate
{
    public class ChangePublishingDbContext : ADbContextWithCustomDbSet, IChangePublishingDbContext
    {
        #region implement IChangePublishingDbContext

        /// <summary>
        /// Placeholder for the ChangePublisherFactory
        /// </summary>
        private IChangesPublisherFactory _changePublisherFactory;

        protected IChangesPublisherFactory ChangesPublisherFactory => _changePublisherFactory == null ? _changePublisherFactory = new ChangesPublisherFactory(this) : _changePublisherFactory;

        public IChangesPublisher<EntityType> ChangesPublisher<EntityType>() where EntityType : class, new()
        {
            return ChangesPublisherFactory.Entity<EntityType>();
        }

        #endregion

        #region Implement abstract DbContextWithCustomDbSet

        protected override string _customDbSetClassName => "IChangePublishingDbSet";

        public override IDbSet<EntityType> GetCustomDbSet<EntityType>(DbSet<EntityType> dbSet)
        {
            return new ChangePublishingDbSet<EntityType>(this, dbSet, ChangesPublisherFactory.Entity<EntityType>());
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
