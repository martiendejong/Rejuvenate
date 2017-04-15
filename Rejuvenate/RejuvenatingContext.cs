using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Rejuvenate
{
    public abstract partial class RejuvenatingDbContext : DbContext, IRejuvenatingDbContext
    {
        #region Public

        #region Convenience

        /// <summary>
        /// Shortcut for generating an IEntityRejuvenator that can be used in the EntityRejuvenators declaration
        /// </summary>
        public IEntityRejuvenator GetRejuvenator<Entity>() where Entity : class, new()
        {
            return new EntityRejuvenator<Entity>(ChangeTracker, this);
        }

        #endregion

        #region Abstract

        /// <summary>
        /// Declare an IEntityRejuvenator instance per entity type using the function GetRejuvenator.
        /// Example:
        /// return new List<IEntityRejuvenator>
        /// {
        ///     GetRejuvenator<MyEntity>()
        /// };
        /// </summary>
        protected abstract List<IEntityRejuvenator> EntityRejuvenators { get; }

        #endregion

        #region Constructors

        ///<summary>Same as with DbContext</summary>
        public RejuvenatingDbContext() : base() { }

        ///<summary>Same as with DbContext</summary>
        public RejuvenatingDbContext(DbCompiledModel model) : base(model) { }

        ///<summary>Same as with DbContext</summary>
        public RejuvenatingDbContext(string nameOrconnectionString) : base(nameOrconnectionString) { }

        ///<summary>Same as with DbContext</summary>
        public RejuvenatingDbContext(string nameOrConnectiongString, DbCompiledModel model) : base(nameOrConnectiongString, model) { }

        ///<summary>Same as with DbContext</summary>
        public RejuvenatingDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }

        ///<summary>Same as with DbContext</summary>
        public RejuvenatingDbContext(ObjectContext objectContext, bool contextOwnsObjectContext) : base(objectContext, contextOwnsObjectContext) { }

        ///<summary>Same as with DbContext</summary>
        public RejuvenatingDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { }

        #endregion

        #region SaveChanges Overrides

        ///<summary>Same as with DbContext, but rejuvenates the items on the client.</summary>
        public override Task<int> SaveChangesAsync()
        {
            PrepareRejuvenation();
            Task<int> task = base.SaveChangesAsync();
            var awaiter = task.GetAwaiter();
            awaiter.OnCompleted(Rejuvenate);
            return task;
        }

        ///<summary>Same as with DbContext, but rejuvenates the items on the client.</summary>
        public override int SaveChanges()
        {
            PrepareRejuvenation();
            var res = base.SaveChanges();
            Rejuvenate();
            return res;
        }

        #endregion

        #endregion

        private IClientRejuvenator<EntityType> GetClientRejuvenator<EntityType, HubType>(Expression<Func<EntityType, bool>> expression) where HubType : IHub where EntityType : class
        {
            if (!ClientRejuvenators.ContainsKey(typeof(EntityType)))
                return null;
            return ClientRejuvenators[typeof(EntityType)]
                .Select(r => (IClientRejuvenator<EntityType>)r)
                .SingleOrDefault(r => LambdaCompare.Eq(r.Expression, expression) && r.Rejuvenate.Method.DeclaringType.Equals(typeof(SignalRHubRejuvenator<HubType>)));
        }

        private IClientRejuvenator<EntityType> GetClientRejuvenator<EntityType>(Expression<Func<EntityType, bool>> expression, RejuvenateClientCallback<EntityType> rejuvenate) where EntityType : class
        {
            if (!ClientRejuvenators.ContainsKey(typeof(EntityType)))
                return null;
            return ClientRejuvenators[typeof(EntityType)]
                .Select(r => (IClientRejuvenator<EntityType>)r)
                .SingleOrDefault(r => LambdaCompare.Eq(r.Expression, expression) && r.Rejuvenate == rejuvenate);
        }

        #region Private

        #region Properties

        private Dictionary<object, Dictionary<EntityState, List<KeyValuePair<DbEntityEntry, object>>>> EntriesByRejuvenatorAndState = new Dictionary<object, Dictionary<EntityState, List<KeyValuePair<DbEntityEntry, object>>>>();

        private Dictionary<Type, List<object>> ClientRejuvenators = new Dictionary<Type, List<object>>();

        #endregion

        #region Methods

        #region Prepare Rejuvenation

        private IEnumerable<Tuple<object, object>> Relations;

        private void PrepareRejuvenation()
        {
            ChangeTracker.DetectChanges();
            // todo do something with relations
            // 2nd item is the list that is being updasted
            // first item is in the list twice
            // todo find the distinct items and check them in preparerejuvenation
            //Relations = this.GetRelationships();
            EntityRejuvenators.ForEach(e => e.PrepareRejuvenation());
        }

        private void PrepareRejuvenation<EntityType>() where EntityType : class, new()
        {
            var itemEntries = ChangeTracker.Entries<EntityType>();
            var changed = itemEntries.Where(e => e.State != EntityState.Unchanged);
            //var entities = changed.Select(c => c.Entity);
            if(changed.Any())
                PrepareRejuvenation(changed);
        }

        private void PrepareRejuvenation<EntityType>(IEnumerable<DbEntityEntry<EntityType>> entries) where EntityType : class, new()
        {
            if (!ClientRejuvenators.ContainsKey(typeof(EntityType))) return;
            foreach (var untypedClientRejuvenator in ClientRejuvenators[typeof(EntityType)])
            {
                var clientRejuvenator = (IClientRejuvenator<EntityType>)untypedClientRejuvenator;
                var pollingStates = new[] { EntityState.Added, EntityState.Deleted, EntityState.Modified };

                var pairs = entries.Where(e => e.State == EntityState.Modified).Select(e => new KeyValuePair<EntityType, EntityType>(e.Entity, CreateWithValues<EntityType>(e.OriginalValues))).ToList();
                var originalEntities = pairs.Select(p => p.Value);

                var applicableOriginalEntities = clientRejuvenator.Expression == null 
                    ? originalEntities.AsQueryable().ToList()
                    : originalEntities.AsQueryable().Where(clientRejuvenator.Expression).ToList();

                var changedEntities = entries.Where(e => pollingStates.Contains(e.State)).Select(i => i.Entity).OfType<EntityType>().AsQueryable();
                var applicableChangedEntities =
                    clientRejuvenator.Expression == null
                        ? changedEntities.ToList()
                        : changedEntities.Where(clientRejuvenator.Expression).ToList();

                var applicableChangedEntries = entries.Where(e => applicableChangedEntities.Contains(e.Entity));

                foreach (var entry in applicableChangedEntries)
                {
                    var state = entry.State;

                    // handle entities that have moved into the query
                    EntityType original = null;
                    if (state == EntityState.Modified)
                    {
                        var org = pairs.Single(p => p.Key == entry.Entity).Value;
                        if (!applicableOriginalEntities.Contains(org))
                        {
                            state = EntityState.Added;
                        }
                        else
                        {
                            original = org;
                        }
                    }

                    if (!EntriesByRejuvenatorAndState.ContainsKey(untypedClientRejuvenator))
                        EntriesByRejuvenatorAndState[clientRejuvenator] = new Dictionary<EntityState, List<KeyValuePair<DbEntityEntry, object>>>();
                    if (!EntriesByRejuvenatorAndState[clientRejuvenator].ContainsKey(state))
                        EntriesByRejuvenatorAndState[clientRejuvenator][state] = new List<KeyValuePair<DbEntityEntry, object>>();
                    EntriesByRejuvenatorAndState[clientRejuvenator][state].Add(new KeyValuePair<DbEntityEntry, object>(entry, original));
                }

                // handle entities that have moved out of the query
                foreach (var entity in applicableOriginalEntities)
                {
                    var changed = pairs.Single(p => p.Value == entity).Key;

                    // if the changed entity is not in the applicable list anymore mark it as deleted
                    if (!applicableChangedEntities.Contains(changed))
                    {
                        var entry = entries.Single(e => e.Entity == changed);
                        var state = EntityState.Deleted;
                        if (!EntriesByRejuvenatorAndState.ContainsKey(untypedClientRejuvenator))
                            EntriesByRejuvenatorAndState[clientRejuvenator] = new Dictionary<EntityState, List<KeyValuePair<DbEntityEntry, object>>>();
                        if (!EntriesByRejuvenatorAndState[clientRejuvenator].ContainsKey(state))
                            EntriesByRejuvenatorAndState[clientRejuvenator][state] = new List<KeyValuePair<DbEntityEntry, object>>();
                        EntriesByRejuvenatorAndState[clientRejuvenator][state].Add(new KeyValuePair<DbEntityEntry, object>(entry, null));
                    }
                }
            }
        }

        #endregion

        #region Rejuvenate

        private void Rejuvenate()
        {
            EntityRejuvenators.ForEach(e => e.Rejuvenate());
        }

        private void Rejuvenate<EntityType>() where EntityType : class
        {
            foreach (var rejuvenatorEntryPair in EntriesByRejuvenatorAndState.Where(entry => entry.Key is IClientRejuvenator<EntityType>))
            {
                var clientRejuvenator = rejuvenatorEntryPair.Key as IClientRejuvenator<EntityType>;
                foreach (var stateEntryPair in rejuvenatorEntryPair.Value)
                {
                    var state = stateEntryPair.Key;
                    clientRejuvenator.Rejuvenate(typeof(EntityType), clientRejuvenator.Id, state, stateEntryPair.Value.Select(i => new KeyValuePair<EntityType, EntityType>((EntityType)i.Key.Entity, (EntityType)i.Value)));
                }
                EntriesByRejuvenatorAndState[clientRejuvenator].Clear();
            }
        }

        #endregion

        #region Helper Functions

        private void RegisterClientRejuvenator<T>(IClientRejuvenator<T> rejuvenator) where T : class
        {
            var type = typeof(T);
            if (!ClientRejuvenators.ContainsKey(type))
                ClientRejuvenators[type] = new List<object>();

            AddRejuvenatorId(rejuvenator);

            ClientRejuvenators[type].Add(rejuvenator);
        }

        private void AddRejuvenatorId<T>(IClientRejuvenator<T> rejuvenator) where T : class
        {
            // seed the rejuvenator id per item type
            if (ClientRejuvenators[typeof(T)].Any())
                rejuvenator.Id = ClientRejuvenators[typeof(T)].Max(r => ((IClientRejuvenator<T>)r).Id) + 1;
            else
                rejuvenator.Id = 1;
        }

        private T CreateWithValues<T>(DbPropertyValues values) where T : class, new()
        {
            T entity = new T();
            Type type = typeof(T);

            foreach (var name in values.PropertyNames)
            {
                var property = type.GetProperty(name);
                property.SetValue(entity, values.GetValue<object>(name));
            }

            return entity;
        }

        #endregion

        #endregion

        #endregion
    }
}
