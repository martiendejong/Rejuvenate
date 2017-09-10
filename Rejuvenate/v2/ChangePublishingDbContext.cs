using EFExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Collections.ObjectModel;
using Microsoft.AspNet.SignalR.Hubs;
using Rejuvenate.v2.EntityChangePublishing;
using Rejuvenate.v2.Internal;

namespace Rejuvenate.v2
{
    public class ChangePublishingDbContext : DbContextWitHub
    {
        public SignalRHubListeners SignalR = new SignalRHubListeners();

        // EventListeners
        protected EntitiesChangedListeners _listeners;
        public EntitiesChangedListeners Listeners
        {
            get
            {
                return _listeners == null ? _listeners = new EntitiesChangedListeners(this) : _listeners;
            }
        }

        // Convenience method for declaring a Queryable
        new public ChangePublishingDbSet<EntityType> Set<EntityType>() where EntityType : class, new()
        {
            return new ChangePublishingDbSet<EntityType>(base.Set<EntityType>(), this);
        }

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
