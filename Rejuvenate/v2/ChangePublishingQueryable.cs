using EFExtensions;
using Microsoft.AspNet.SignalR.Hubs;
using Rejuvenate.v2;
using Rejuvenate.v2.Internal;
using Rejuvenate.v2.SignalRChangePublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2
{
    public class ChangePublishingQueryable<EntityType> : IQueryable<EntityType> where EntityType : class, new()
    {
        #region Constructors

        public ChangePublishingQueryable(IQueryable<EntityType> originalQueryable, ChangePublishingDbContext dbContext)
        {
            InnerQueryable = originalQueryable;
            DbContext = dbContext;
        }

        public ChangePublishingQueryable(IQueryable<EntityType> originalQueryable, ChangePublishingDbContext dbContext, Expression<Func<EntityType, bool>> expression) : this(originalQueryable, dbContext)
        {
            InnerExpression = expression;
        }

        #endregion

        #region Subscribe

        // Subscribe an event handler
        public EntitiesChangedListener<EntityType> Subscribe(EntitiesChangedHandler<EntityType> handler)
        {
            return DbContext.Listeners.GetListener(InnerExpression, handler);
        }

        #endregion

        #region Subscribe Linked Entity

        public void SubscribeChildEntity<ChildEntityType, PrimaryKeyType>(Func<ChildEntityType, PrimaryKeyType> pathToForeignKey, Func<PrimaryKeyType, EntityType> getEntityByPrimaryKey, EntitiesChangedListener<EntityType> channel) where ChildEntityType : class, new()
        {
            SubscribeChildEntity(pathToForeignKey, getEntityByPrimaryKey, channel, (messages, childEntityChannel) => channel.Receive(messages));
        }

        public void SubscribeChildEntity<ChildEntityType, PrimaryKeyType>(Func<ChildEntityType, PrimaryKeyType> pathToForeignKey, Func<PrimaryKeyType, EntityType> getEntityByPrimaryKey, EntitiesChangedListener<EntityType> channel, EntitiesChangedHandler<EntityType> handler) where ChildEntityType : class, new()
        {
            var childHandler = new LinkedEntityChangedHandler<ChildEntityType, EntityType, PrimaryKeyType>()
            {
                PathToForeignKey = pathToForeignKey,
                GetEntityByPrimaryKey = getEntityByPrimaryKey,
                DbContext = DbContext,
                Channel = channel,
                Handler = handler
            };
            DbContext.Hub.Subscribe<ChildEntityType>(childHandler.Receive);
        }

        #endregion

        #region SignalR

        // Subscribe a SignalR hubtype
        public EntitiesChangedListener<EntityType> Subscribe<HubType>() where HubType : IHub
        {
            SignalRHubListener<HubType> publisher = DbContext.SignalR.GetListener<HubType>();

            return Subscribe(publisher.Receive);
        }

        public void SubscribeChildEntity<ChildEntityType, PrimaryKeyType, HubType>(Func<ChildEntityType, PrimaryKeyType> pathToForeignKey, Func<PrimaryKeyType, EntityType> getEntityByPrimaryKey, EntitiesChangedListener<EntityType> channel) where HubType : IHub where ChildEntityType : class, new()
        {
            SignalRHubListener<HubType> publisher = DbContext.SignalR.GetListener<HubType>();

            SubscribeChildEntity(pathToForeignKey, getEntityByPrimaryKey, channel, publisher.Receive);
        }

        #endregion

        #region overrides

        public ChangePublishingQueryable<EntityType> Where(Expression<Func<EntityType, bool>> expression)
        {
            return new ChangePublishingQueryable<EntityType>(InnerQueryable.Where(expression), DbContext, And(expression));
        }

        #endregion

        #region Implement IQueryable Proxy

        public Type ElementType
        {
            get
            {
                return InnerQueryable.ElementType;
            }
        }

        public Expression Expression
        {
            get
            {
                return InnerQueryable.Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return InnerQueryable.Provider;
            }
        }

        public IEnumerator<EntityType> GetEnumerator()
        {
            return InnerQueryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerQueryable.GetEnumerator();
        }

        #endregion

        #region protected

        protected IQueryable<EntityType> InnerQueryable;

        protected Expression<Func<EntityType, bool>> InnerExpression;

        protected ChangePublishingDbContext DbContext;

        protected Expression<Func<EntityType, bool>> And(Expression<Func<EntityType, bool>> expression)
        {
            return InnerExpression == null ? expression : InnerExpression.And(expression);
        }

        #endregion
    }
}
