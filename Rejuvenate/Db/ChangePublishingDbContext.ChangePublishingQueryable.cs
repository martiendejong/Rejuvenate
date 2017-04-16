using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.Entity.Infrastructure;
using Rejuvenate.Db.SignalR;
using Rejuvenate.Db.Helpers;

namespace Rejuvenate.Db
{
    public abstract partial class ChangePublishingDbContext
    {
        public class ChangePublishingQueryable<EntityType> : IChangePublishingQueryable<EntityType> where EntityType : class
        {
            #region Public

            /// <summary>
            /// Instantiates a query that that monitors and publishes changes.
            /// </summary>
            /// <param name="dbSet">The DbSet where the query originates.</param>
            /// <param name="dbContext">The DbContext where the query will be executed.</param>
            public ChangePublishingQueryable(IDbSet<EntityType> dbSet, ChangePublishingDbContext dbContext) : this((IQueryable<EntityType>)dbSet, dbContext) { }

            public IChangePublishingQueryable<EntityType> Where(Expression<Func<EntityType, bool>> expression)
            {
                return new ChangePublishingQueryable<EntityType>(OriginalQueryable.Where(expression), DbContext, And(expression));
            }

            public IChangePublisher<EntityType> Subscribe(EntitiesChangedHandler<EntityType> clientCallback)
            {
                var publisher = DbContext.GetPublisher(Expression, clientCallback);
                if (publisher == null)
                {
                    publisher = new ChangePublisher<EntityType>();
                    publisher.Query = Expression;
                    publisher.EntitiesChanged = clientCallback;
                    DbContext.RegisterPublisher(publisher);
                }
                return publisher;
            }

            public IChangePublisher<EntityType> Subscribe<HubType>() where HubType : IHub
            {
                var publisher = DbContext.GetPublisher<EntityType, HubType>(Expression);
                if (publisher == null)
                {
                    publisher = new ChangePublisher<EntityType>();
                    publisher.Query = Expression;
                    var signalRHubPublisher = new SignalRHubPublisher<HubType>();
                    publisher.EntitiesChanged = signalRHubPublisher.Publish;
                    DbContext.RegisterPublisher(publisher);
                }
                return publisher;
            }


            public IChangePublisher<LinkedEntityType> SubscribeLinkedEntity<LinkedEntityType, HubType, EntityIdType>(
                IChangePublishingQueryable<LinkedEntityType> linkedEntities, 
                Expression<Func<LinkedEntityType, EntityType>> select, 
                Expression<Func<LinkedEntityType, EntityIdType>> foreignKeySelect, 
                Func<IQueryable<EntityIdType>, IQueryable<EntityType>> resolveEntityById,
                int publisherId) where LinkedEntityType : class where HubType : IHub
            {
                var pub = DbContext.GetLinkedEntityPublisher<EntityType, LinkedEntityType, HubType, EntityIdType>(Expression, select, foreignKeySelect, resolveEntityById, publisherId);
                if(pub == null)
                {
                    // TODO make this ' normal'
                    pub = new LinkedEntityChangedHandler<EntityType, LinkedEntityType, HubType, EntityIdType>()
                    {
                        Expression = Expression,
                        select = select,
                        publisherId = publisherId,
                        resolveEntityById = resolveEntityById,
                        foreignKeySelect = foreignKeySelect,
                    };
                    DbContext.LinkedEntityPublishers.Add(pub);
                    return linkedEntities.Subscribe(pub.ChangedHandler);
                }
                // TODO get publisher
                return null;
            }

            public IQueryable<EntityType> AsQueryable()
            {
                return OriginalQueryable;
            }

            #endregion

            #region Protected

            protected IQueryable<EntityType> OriginalQueryable;

            protected ChangePublishingDbContext DbContext;

            protected Expression<Func<EntityType, bool>> Expression;

            #region Constructors

            protected ChangePublishingQueryable(IQueryable<EntityType> originalQueryable, ChangePublishingDbContext dbContext)
            {
                OriginalQueryable = originalQueryable;
                DbContext = dbContext;
            }

            protected ChangePublishingQueryable(IQueryable<EntityType> originalQueryable, ChangePublishingDbContext dbContext, Expression<Func<EntityType, bool>> expression) : this(originalQueryable, dbContext)
            {
                Expression = expression;
            }

            #endregion

            protected Expression<Func<EntityType, bool>> And(Expression<Func<EntityType, bool>> expression)
            {
                return Expression == null ? expression : Expression.And(expression);
            }

            #endregion
        }

        public class LinkedEntityChangedHandler<EntityType, LinkedEntityType, HubType, EntityIdType> where LinkedEntityType : class where HubType : IHub
        {
            public Expression<Func<EntityType, bool>> Expression;

            public Expression<Func<LinkedEntityType, EntityType>> select;

            public int publisherId;

            public Func<IQueryable<EntityIdType>, IQueryable<EntityType>> resolveEntityById;

            public Expression<Func<LinkedEntityType, EntityIdType>> foreignKeySelect;

            public void ChangedHandler(Type type, int rejuvenatorId, EntityState state, IEnumerable<KeyValuePair<LinkedEntityType, LinkedEntityType>> subEntityPairs)
            {
                var signalRHubPublisher = new SignalRHubPublisher<HubType>();

                // rejuvenate the entities that are linked to the subentity
                var entities = subEntityPairs.Select(entity => entity.Key).AsQueryable().Select(select);
                entities = (Expression == null ? entities : entities.Where(Expression)).Distinct();
                signalRHubPublisher.Publish(type, publisherId, EntityState.Modified, entities);

                // rejuvenate the entities that are unlinked from the subentity
                var originalSubEntities = subEntityPairs.Where(pair => pair.Value != null).Select(pair => pair.Value);
                var originalEntityIds = originalSubEntities.AsQueryable().Where(entity => entity != null).Select(foreignKeySelect).Distinct();
                var updatedOrgEntities = resolveEntityById(originalEntityIds);
                updatedOrgEntities = (Expression == null ? updatedOrgEntities : updatedOrgEntities.Where(Expression));
                signalRHubPublisher.Publish(type, publisherId, EntityState.Modified, updatedOrgEntities);
            }
        }
    }
}