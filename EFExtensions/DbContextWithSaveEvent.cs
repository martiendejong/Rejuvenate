using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFExtensions
{
    public class DbContextWithSaveEvent : DbContext
    {
        public delegate void SaveStartHandler(DbContext context);

        public delegate void SaveCompletedHandler(DbContext context);

        public event SaveStartHandler SaveStart;

        public event SaveStartHandler SaveCompleted;

        public override Task<int> SaveChangesAsync()
        {
            SaveStart(this);
            var task = base.SaveChangesAsync();
            task.GetAwaiter().OnCompleted(() => { SaveCompleted(this); });
            return task;
        }

        public override int SaveChanges()
        {
            SaveStart(this);
            var res = base.SaveChanges();
            SaveCompleted(this);
            return res;
        }

        #region Inherit constructors from the base class

        ///<summary>Same as with DbContext</summary>
        public DbContextWithSaveEvent() : base() { }

        ///<summary>Same as with DbContext</summary>
        public DbContextWithSaveEvent(DbCompiledModel model) : base(model) { }

        ///<summary>Same as with DbContext</summary>
        public DbContextWithSaveEvent(string nameOrconnectionString) : base(nameOrconnectionString) { }

        ///<summary>Same as with DbContext</summary>
        public DbContextWithSaveEvent(string nameOrConnectiongString, DbCompiledModel model) : base(nameOrConnectiongString, model) { }

        ///<summary>Same as with DbContext</summary>
        public DbContextWithSaveEvent(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }

        ///<summary>Same as with DbContext</summary>
        public DbContextWithSaveEvent(ObjectContext objectContext, bool contextOwnsObjectContext) : base(objectContext, contextOwnsObjectContext) { }

        ///<summary>Same as with DbContext</summary>
        public DbContextWithSaveEvent(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { }

        #endregion
    }
}
