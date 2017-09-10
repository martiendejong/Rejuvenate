using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFExtensions;
using Rejuvenate.v2.EntityChangePublishing;

namespace Rejuvenate.v2.Internal
{
    public class DbContextWitHub : DbContextWithSaveEvent
    {
        public DbContextChangesHub Hub = new DbContextChangesHub();

        public virtual void Init()
        {
            SaveStart += Hub.GatherChanges;
            SaveCompleted += Hub.PublishChanges;
        }

        #region Inherit constructors from the base class and call the Init function

        ///<summary>Same as with DbContext</summary>
        public DbContextWitHub() : base() { Init(); }

        ///<summary>Same as with DbContext</summary>
        public DbContextWitHub(DbCompiledModel model) : base(model) { Init(); }

        ///<summary>Same as with DbContext</summary>
        public DbContextWitHub(string nameOrconnectionString) : base(nameOrconnectionString) { Init(); }

        ///<summary>Same as with DbContext</summary>
        public DbContextWitHub(string nameOrConnectiongString, DbCompiledModel model) : base(nameOrConnectiongString, model) { Init(); }

        ///<summary>Same as with DbContext</summary>
        public DbContextWitHub(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { Init(); }

        ///<summary>Same as with DbContext</summary>
        public DbContextWitHub(ObjectContext objectContext, bool contextOwnsObjectContext) : base(objectContext, contextOwnsObjectContext) { Init(); }

        ///<summary>Same as with DbContext</summary>
        public DbContextWitHub(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { Init(); }

        #endregion
    }
}