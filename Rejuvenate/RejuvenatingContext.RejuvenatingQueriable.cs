using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;

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

            #endregion
        }
    }
}