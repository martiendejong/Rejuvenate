using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.Db
{
    public interface IChangePublishingQueryable<EntityType> where EntityType : class
    {

        /// <summary>
        /// Same as with IQueryable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IChangePublishingQueryable<EntityType> Where(Expression<Func<EntityType, bool>> expression);

        //IClientRejuvenator<IncludedEntityType> RejuvenateInclude<IncludedEntityType, HubType>(IRejuvenatingQueryable<IncludedEntityType> includedEntitiesQuery, Expression<Func<IncludedEntityType, EntityType>> select, Expression<Func<EntityType, object>> selectId, int rejuvenatorId) where IncludedEntityType : class where HubType : IHub;
        IChangePublisher<IncludedEntityType> SubscribeLinkedEntity<IncludedEntityType, HubType, IdType>(IChangePublishingQueryable<IncludedEntityType> includedEntitiesQuery, Expression<Func<IncludedEntityType, EntityType>> select, Expression<Func<IncludedEntityType, IdType>> includedEntity_foreignKeySelector, Func<IQueryable<IdType>, IQueryable<EntityType>> getOriginalEntities, int rejuvenatorId) where IncludedEntityType : class where HubType : IHub;

        // todo better thinking
        //IRejuvenatingQueryable<EntityType> Include<TProperty>(Expression<Func<EntityType, TProperty>> propertyExpression);

        /// <summary>
        /// Adds an agent to the query that publishes changes to a callback function. Then returns the query.
        /// </summary>
        /// <param name="clientCallback">The callback function that is be called when publishing the changed items.</param>
        /// <returns>The generated IClientRejuvenator.</returns>
        IChangePublisher<EntityType> Subscribe(EntitiesChangedHandler<EntityType> publish);

        /// <summary>
        /// Adds an agent to the query that publishes changes to the SignalR clients. Then returns the query.
        /// </summary>
        /// <returns>The generated IClientRejuvenator.</returns>
        IChangePublisher<EntityType> Subscribe<HubType>() where HubType : IHub;

        /// <summary>
        /// Get the original LINQ IQueryable object
        /// </summary>
        /// <returns>The original Queryable.</returns>
        IQueryable<EntityType> AsQueryable();
    }
}
