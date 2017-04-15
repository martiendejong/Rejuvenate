using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.Entity.Infrastructure;

namespace Rejuvenate
{
    public abstract partial class RejuvenatingDbContext
    {
        public class RejuvenatingQueryable<EntityType> : IRejuvenatingQueryable<EntityType> where EntityType : class
        {
            #region Public

            /// <summary>
            /// Instantiates a query that that monitors and publishes changes.
            /// </summary>
            /// <param name="dbSet">The DbSet where the query originates.</param>
            /// <param name="dbContext">The DbContext where the query will be executed.</param>
            public RejuvenatingQueryable(IDbSet<EntityType> dbSet, RejuvenatingDbContext dbContext) : this((IQueryable<EntityType>)dbSet, dbContext) { }

            public IRejuvenatingQueryable<EntityType> Where(Expression<Func<EntityType, bool>> expression)
            {
                return new RejuvenatingQueryable<EntityType>(OriginalQueryable.Where(expression), DbContext, And(expression));
            }

            public IClientRejuvenator<EntityType> RejuvenateQuery(RejuvenateClientCallback<EntityType> clientCallback)
            {
                var rejuvenator = DbContext.GetClientRejuvenator(Expression, clientCallback);
                if (rejuvenator == null)
                {
                    rejuvenator = new ClientRejuvenator<EntityType>();
                    rejuvenator.Expression = Expression;
                    rejuvenator.Rejuvenate = clientCallback;
                    DbContext.RegisterClientRejuvenator(rejuvenator);
                }
                return rejuvenator;
            }

            public IClientRejuvenator<EntityType> RejuvenateQuery<HubType>() where HubType : IHub
            {
                var rejuvenator = DbContext.GetClientRejuvenator<EntityType, HubType>(Expression);
                if (rejuvenator == null)
                {
                    rejuvenator = new ClientRejuvenator<EntityType>();
                    rejuvenator.Expression = Expression;
                    var signalRHubRejuvenator = new SignalRHubRejuvenator<HubType>();
                    rejuvenator.Rejuvenate = signalRHubRejuvenator.Rejuvenate;
                    DbContext.RegisterClientRejuvenator(rejuvenator);
                }
                return rejuvenator;
            }

            public IQueryable<EntityType> AsQueryable()
            {
                return OriginalQueryable;
            }

            #endregion

            #region Protected

            protected IQueryable<EntityType> OriginalQueryable;

            protected RejuvenatingDbContext DbContext;

            protected Expression<Func<EntityType, bool>> Expression;

            #region Constructors

            protected RejuvenatingQueryable(IQueryable<EntityType> originalQueryable, RejuvenatingDbContext dbContext)
            {
                OriginalQueryable = originalQueryable;
                DbContext = dbContext;
            }

            protected RejuvenatingQueryable(IQueryable<EntityType> originalQueryable, RejuvenatingDbContext dbContext, Expression<Func<EntityType, bool>> expression) : this(originalQueryable, dbContext)
            {
                Expression = expression;
            }

            #endregion

            protected Expression<Func<EntityType, bool>> And(Expression<Func<EntityType, bool>> expression)
            {
                return Expression == null ? expression : Expression.And(expression);
            }

            public IClientRejuvenator<IncludedEntityType> RejuvenateInclude<IncludedEntityType, HubType, IdType>(IRejuvenatingQueryable<IncludedEntityType> includedEntitiesQuery, Expression<Func<IncludedEntityType, EntityType>> select, Expression<Func<IncludedEntityType, IdType>> includedEntity_foreignKeySelector, Func<IQueryable<IdType>, IQueryable<EntityType>> getOriginalEntities, int rejuvenatorId) where IncludedEntityType : class where HubType : IHub
            {
                RejuvenateClientCallback<IncludedEntityType> callback = (Type type, int subRejuvenatorId, EntityState state, IEnumerable<KeyValuePair<IncludedEntityType, IncludedEntityType>> subEntityPairs) =>
                {
                    var signalRHubRejuvenator = new SignalRHubRejuvenator<HubType>();

                    // rejuvenate the entities that are linked on the subentity
                    var entities = subEntityPairs.Select(entity => entity.Key).AsQueryable().Select(select);
                    entities = (Expression == null ? entities : entities.Where(Expression)).Distinct();
                    signalRHubRejuvenator.Rejuvenate(type, rejuvenatorId, EntityState.Modified, entities);

                    // rejuvenate the entities that are unlinked on the subentity
                    var originalSubEntities = subEntityPairs.Where(pair => pair.Value != null).Select(pair => pair.Value);
                    var originalEntityIds = originalSubEntities.AsQueryable().Where(entity => entity != null).Select(includedEntity_foreignKeySelector).Distinct();
                    var updatedOrgEntities = getOriginalEntities(originalEntityIds);
                    updatedOrgEntities = (Expression == null ? updatedOrgEntities : updatedOrgEntities.Where(Expression));
                    signalRHubRejuvenator.Rejuvenate(type, rejuvenatorId, EntityState.Modified, updatedOrgEntities);
                };
                return includedEntitiesQuery.RejuvenateQuery(callback);
            }

            /*public IRejuvenatingQueryable<EntityType> Include<IncludedEntityType>(IDbSet<IncludedEntityType> dbSet, Expression<Func<IncludedEntityType, bool>> expression) where IncludedEntityType : class, new()
            {
                var q = new RejuvenatingQueryable<IncludedEntityType>(dbSet, DbContext);
                q.Where(propertyExpression);
                var rejuvenator = DbContext.GetClientRejuvenator<EntityType, HubType>(Expression);
                if (rejuvenator == null)
                {
                    rejuvenator = new ClientRejuvenator<EntityType>();
                    rejuvenator.Expression = Expression;
                    var signalRHubRejuvenator = new SignalRHubRejuvenator<HubType>();
                    rejuvenator.Rejuvenate = signalRHubRejuvenator.Rejuvenate;
                    DbContext.RegisterClientRejuvenator(rejuvenator);
                }
                return rejuvenator;

                typeof(TProperty)
                Expression<Func<EntityType, TProperty>> x = null;
                OriginalQueryable.Include( x);
            }*/

            #endregion
        }
    }
}