using EFExtensions;
using Microsoft.AspNet.SignalR.Hubs;
using Rejuvenate.Db.Helpers;
using Rejuvenate.Db.SignalR;
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

namespace Rejuvenate.Db
{
    public abstract partial class ChangePublishingDbContext : DbContext, IChangePublishingDbContext
    {
        #region Public

        #region Convenience

        /// <summary>
        /// Shortcut for generating an IEntityRejuvenator that can be used in the EntityRejuvenators declaration
        /// </summary>
        public IChangeProcessor GetChangeProcessor<Entity>() where Entity : class, new()
        {
            return new ChangeProcessor<Entity>(this);
        }

        #endregion

        #region Abstract

        /// <summary>
        /// Declare an IChangeProcessor instance per entity type using the function GetChangeProcessor.
        /// Example:
        /// return new List<IChangeProcessor>
        /// {
        ///     GetChangeProcessor<MyEntity>()
        /// };
        /// </summary>
        protected abstract List<IChangeProcessor> ChangeProcessors { get; }

        #endregion

        #region Constructors

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

        #region SaveChanges Overrides

        ///<summary>Same as with DbContext, but rejuvenates the items on the client.</summary>
        public override Task<int> SaveChangesAsync()
        {
            GatherChanges();
            Task<int> task = base.SaveChangesAsync();
            var awaiter = task.GetAwaiter();
            awaiter.OnCompleted(Publish);
            return task;
        }

        ///<summary>Same as with DbContext, but rejuvenates the items on the client.</summary>
        public override int SaveChanges()
        {
            GatherChanges();
            var res = base.SaveChanges();
            Publish();
            return res;
        }

        #endregion

        #endregion

        private IChangePublisher<EntityType> GetPublisher<EntityType, HubType>(Expression<Func<EntityType, bool>> expression) where HubType : IHub where EntityType : class
        {
            if (!Publishers.ContainsKey(typeof(EntityType)))
                return null;
            return Publishers[typeof(EntityType)]
                .Select(r => (IChangePublisher<EntityType>)r)
                .SingleOrDefault(pub => LambdaCompare.Eq(pub.Query, expression) && pub.EntitiesChanged.Method.DeclaringType.Equals(typeof(SignalRHubPublisher<HubType>)));
        }

        private IChangePublisher<EntityType> GetPublisher<EntityType>(Expression<Func<EntityType, bool>> expression, EntitiesChangedHandler<EntityType> entitiesChanged) where EntityType : class
        {
            if (!Publishers.ContainsKey(typeof(EntityType)))
                return null;
            return Publishers[typeof(EntityType)]
                .Select(pub => (IChangePublisher<EntityType>)pub)
                .SingleOrDefault(pub => LambdaCompare.Eq(pub.Query, expression) && pub.EntitiesChanged == entitiesChanged);
        }

        private LinkedEntityChangedHandler<EntityType, LinkedEntityType, HubType, EntityIdType> GetLinkedEntityPublisher<EntityType, LinkedEntityType, HubType, EntityIdType>(
            Expression<Func<EntityType, bool>> expression,
            Expression<Func<LinkedEntityType, EntityType>> select,
            Expression<Func<LinkedEntityType, EntityIdType>> foreignKeySelect,
            Func<IQueryable<EntityIdType>, IQueryable<EntityType>> resolveEntityById,
            int publisherId) where EntityType : class where LinkedEntityType : class where HubType : IHub
        {
            return LinkedEntityPublishers.Select(pub => pub as LinkedEntityChangedHandler<EntityType, LinkedEntityType, HubType, EntityIdType>)
                .SingleOrDefault(
                    pub => pub != null
                    && pub.publisherId == publisherId
                    && LambdaCompare.Eq(pub.select, select)
                    && LambdaCompare.Eq(pub.foreignKeySelect, foreignKeySelect)
                    && LambdaCompare.Eq(pub.Expression, expression)
                );
        }

        #region Private

        #region Properties

        private Dictionary<object, Dictionary<EntityState, List<KeyValuePair<DbEntityEntry, object>>>> EntriesByPublisherAndState = new Dictionary<object, Dictionary<EntityState, List<KeyValuePair<DbEntityEntry, object>>>>();

        private Dictionary<Type, List<object>> Publishers = new Dictionary<Type, List<object>>();

        private List<object> LinkedEntityPublishers = new List<object>();

        #endregion

        #region Methods

        #region Prepare Rejuvenation

        private IEnumerable<Tuple<object, object>> Relations;

        private void GatherChanges()
        {
            ChangeTracker.DetectChanges();
            ChangeProcessors.ForEach(e => e.ProcessChanges());
        }

        private void GatherChanges<EntityType>() where EntityType : class, new()
        {
            var itemEntries = ChangeTracker.Entries<EntityType>();
            var changed = itemEntries.Where(e => e.State != EntityState.Unchanged);
            if(changed.Any())
                GatherChanges(changed);
        }

        private void GatherChanges<EntityType>(IEnumerable<DbEntityEntry<EntityType>> entries) where EntityType : class, new()
        {
            if (!Publishers.ContainsKey(typeof(EntityType))) return;
            foreach (var untypedClientRejuvenator in Publishers[typeof(EntityType)])
            {
                var clientRejuvenator = (IChangePublisher<EntityType>)untypedClientRejuvenator;
                var pollingStates = new[] { EntityState.Added, EntityState.Deleted, EntityState.Modified };

                var pairs = entries.Where(e => e.State == EntityState.Modified).Select(e => new KeyValuePair<EntityType, EntityType>(e.Entity, CreateWithValues<EntityType>(e.OriginalValues))).ToList();
                var originalEntities = pairs.Select(p => p.Value);

                var applicableOriginalEntities = clientRejuvenator.Query == null 
                    ? originalEntities.AsQueryable().ToList()
                    : originalEntities.AsQueryable().Where(clientRejuvenator.Query).ToList();

                var changedEntities = entries.Where(e => pollingStates.Contains(e.State)).Select(i => i.Entity).OfType<EntityType>().AsQueryable();
                var applicableChangedEntities =
                    clientRejuvenator.Query == null
                        ? changedEntities.ToList()
                        : changedEntities.Where(clientRejuvenator.Query).ToList();

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

                    if (!EntriesByPublisherAndState.ContainsKey(untypedClientRejuvenator))
                        EntriesByPublisherAndState[clientRejuvenator] = new Dictionary<EntityState, List<KeyValuePair<DbEntityEntry, object>>>();
                    if (!EntriesByPublisherAndState[clientRejuvenator].ContainsKey(state))
                        EntriesByPublisherAndState[clientRejuvenator][state] = new List<KeyValuePair<DbEntityEntry, object>>();
                    EntriesByPublisherAndState[clientRejuvenator][state].Add(new KeyValuePair<DbEntityEntry, object>(entry, original));
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
                        if (!EntriesByPublisherAndState.ContainsKey(untypedClientRejuvenator))
                            EntriesByPublisherAndState[clientRejuvenator] = new Dictionary<EntityState, List<KeyValuePair<DbEntityEntry, object>>>();
                        if (!EntriesByPublisherAndState[clientRejuvenator].ContainsKey(state))
                            EntriesByPublisherAndState[clientRejuvenator][state] = new List<KeyValuePair<DbEntityEntry, object>>();
                        EntriesByPublisherAndState[clientRejuvenator][state].Add(new KeyValuePair<DbEntityEntry, object>(entry, null));
                    }
                }
            }
        }

        #endregion

        #region Rejuvenate

        private void Publish()
        {
            ChangeProcessors.ForEach(e => e.Publish());
        }

        private void Publish<EntityType>() where EntityType : class
        {
            foreach (var publisherEntryPair in EntriesByPublisherAndState.Where(entry => entry.Key is IChangePublisher<EntityType>))
            {
                var publisher = publisherEntryPair.Key as IChangePublisher<EntityType>;
                foreach (var stateEntryPair in publisherEntryPair.Value)
                {
                    var state = stateEntryPair.Key;
                    publisher.EntitiesChanged(typeof(EntityType), publisher.Id, state, stateEntryPair.Value.Select(newAndOrgEntity => new KeyValuePair<EntityType, EntityType>((EntityType)newAndOrgEntity.Key.Entity, (EntityType)newAndOrgEntity.Value)));
                }
                EntriesByPublisherAndState[publisher].Clear();
            }
        }

        #endregion

        #region Helper Functions

        private void RegisterPublisher<EntityType>(IChangePublisher<EntityType> publisher) where EntityType : class
        {
            var type = typeof(EntityType);
            if (!Publishers.ContainsKey(type))
                Publishers[type] = new List<object>();

            AddPublisherId(publisher);

            Publishers[type].Add(publisher);
        }

        private void AddPublisherId<EntityType>(IChangePublisher<EntityType> publisher) where EntityType : class
        {
            // seed the rejuvenator id per item type
            if (Publishers[typeof(EntityType)].Any())
                publisher.Id = Publishers[typeof(EntityType)].Max(r => ((IChangePublisher<EntityType>)r).Id) + 1;
            else
                publisher.Id = 1;
        }

        /*
         * Creates an entity from the given values
         */
        private EntityType CreateWithValues<EntityType>(DbPropertyValues values) where EntityType : class, new()
        {
            EntityType entity = new EntityType();
            Type type = typeof(EntityType);

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
