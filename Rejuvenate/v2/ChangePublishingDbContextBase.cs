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

namespace Rejuvenate.v2
{
    public delegate void EntitiesChangedHandler<EntityType>(IEnumerable<EntityChangedMessage<EntityType>> entities);

    public class EntityChangedMessage<EntityType>
    {
        public EntityChangedMessage(EntityState entityState, EntityType oldEntity, EntityType newEntity)
        {
            EntityState = entityState;
            OldEntity = oldEntity;
            NewEntity = newEntity;
        }

        public EntityState EntityState;
        public EntityType OldEntity;
        public EntityType NewEntity;
    }

    public interface IChangePublisher
    {
        void GatherChanges(DbChangeTracker changeTracker);

        void PublishChanges();

        void Add(object handler);
    }

    public class DbChangePublisherFor<EntityType> : IChangePublisher where EntityType : class, new()
    {
        public List<EntitiesChangedHandler<EntityType>> Handlers = new List<EntitiesChangedHandler<EntityType>>();

        public IEnumerable<EntityChangedMessage<EntityType>> GatheredChanges;

        public void Add(object handler)
        {
            Handlers.Add((EntitiesChangedHandler<EntityType>)handler);
        }

        public void GatherChanges(DbChangeTracker changeTracker)
        {
            var entries = changeTracker.Entries<EntityType>();
            GatheredChanges = entries.Select(GetEntityChangedMessage).ToList().Where(m => m != null);
        }

        public void PublishChanges()
        {
            Handlers.ForEach(h => h(GatheredChanges));
        }

        public static EntityChangedMessage<EntityType> GetEntityChangedMessage(DbEntityEntry<EntityType> entry)
        {
            switch(entry.State)
            {
                case EntityState.Added:
                    return GetEntityAddedMessage(entry);
                case EntityState.Deleted:
                    return GetEntityRemovedMessage(entry);
                case EntityState.Modified:
                    return GetEntityModifiedMessage(entry);
                default:
                    return null;
            }
        }

        public static EntityChangedMessage<EntityType> GetEntityAddedMessage(DbEntityEntry<EntityType> entry)
        {
            return new EntityChangedMessage<EntityType>(entry.State, null, entry.Entity);
        }

        public static EntityChangedMessage<EntityType> GetEntityRemovedMessage(DbEntityEntry<EntityType> entry)
        {
            return new EntityChangedMessage<EntityType>(entry.State, entry.Entity, null);
        }

        public static EntityChangedMessage<EntityType> GetEntityModifiedMessage(DbEntityEntry<EntityType> entry)
        {
            return new EntityChangedMessage<EntityType>(entry.State, entry.OriginalValues.ToEntity<EntityType>(), entry.Entity);
        }
    }

    public class DbChangePublisher
    {
        public List<IChangePublisher> Publishers = new List<IChangePublisher>();

        public void Subscribe<EntityType>(EntitiesChangedHandler<EntityType> handler) where EntityType : class, new()
        {
            var SubscriptionsForEntityType = Publishers.OfType<DbChangePublisherFor<EntityType>>().FirstOrDefault();
            if (SubscriptionsForEntityType == null)
            {
                SubscriptionsForEntityType = new DbChangePublisherFor<EntityType>();
                Publishers.Add(SubscriptionsForEntityType);
            }
            SubscriptionsForEntityType.Add(handler);
        }

        public void GatherChanges(DbContext context)
        {
            context.ChangeTracker.DetectChanges();
            Publishers.ForEach(s => s.GatherChanges(context.ChangeTracker));
        }

        public void PublishChanges(DbContext context)
        {
            Publishers.ForEach(s => s.PublishChanges());
        }
    }

    public class ChangePublishingDbContextBase : DbContextWithSaveEvent
    {
        public DbChangePublisher ChangePublisher = new DbChangePublisher();

        public virtual void Init()
        {
            SaveStart += ChangePublisher.GatherChanges;
            SaveCompleted += ChangePublisher.PublishChanges;
        }

        #region Inherit constructors from the base class and call the Init function

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContextBase() : base() { Init(); }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContextBase(DbCompiledModel model) : base(model) { Init(); }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContextBase(string nameOrconnectionString) : base(nameOrconnectionString) { Init(); }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContextBase(string nameOrConnectiongString, DbCompiledModel model) : base(nameOrConnectiongString, model) { Init(); }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContextBase(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { Init(); }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContextBase(ObjectContext objectContext, bool contextOwnsObjectContext) : base(objectContext, contextOwnsObjectContext) { Init(); }

        ///<summary>Same as with DbContext</summary>
        public ChangePublishingDbContextBase(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { Init(); }

        #endregion
    }
}