using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public abstract partial class RejuvenatingDbContext : DbContext
    {
        public class RejuvenatingQueryable<T> : IRejuvenatingQueryable<T> where T : class
        {
            #region Public

            /// <summary>
            /// Instantiates a query that that monitors and publishes changes.
            /// </summary>
            /// <param name="dbSet">The DbSet where the query originates.</param>
            /// <param name="dbContext">The DbContext where the query will be executed.</param>
            public RejuvenatingQueryable(IDbSet<T> dbSet, RejuvenatingDbContext dbContext) : this((IQueryable<T>)dbSet, dbContext) { }

            /// <summary>
            /// Same as with IQueryable
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public IRejuvenatingQueryable<T> Where(Expression<Func<T, bool>> expression)
            {
                return new RejuvenatingQueryable<T>(OriginalQueryable.Where(expression), DbContext, And(expression));
            }

            /// <summary>
            /// Adds an agent to the query that monitors and publishes changes to the client. Then returns the query.
            /// </summary>
            /// <param name="clientCallback">The callback function that is be called when publishing the changed items.</param>
            /// <returns>The internal LINQ query.</returns>
            public IQueryable<T> RejuvenateQuery(RejuvenateClientCallback<T> clientCallback)
            {
                IClientRejuvenator<T> rejuvenator = new ClientRejuvenator<T>();
                rejuvenator.Expression = Expression;
                rejuvenator.Rejuvenate = clientCallback;
                DbContext.RegisterClientRejuvenator(rejuvenator);
                return OriginalQueryable;
            }

            #endregion

            #region Protected

            protected IQueryable<T> OriginalQueryable;

            protected RejuvenatingDbContext DbContext;

            protected Expression<Func<T, bool>> Expression;

            protected RejuvenatingQueryable(IQueryable<T> originalQueryable, RejuvenatingDbContext dbContext)
            {
                OriginalQueryable = originalQueryable;
                DbContext = dbContext;
            }

            protected RejuvenatingQueryable(IQueryable<T> originalQueryable, RejuvenatingDbContext dbContext, Expression<Func<T, bool>> expression) : this(originalQueryable, dbContext)
            {
                Expression = expression;
            }

            protected Expression<Func<T, bool>> And(Expression<Func<T, bool>> expression)
            {
                return Expression == null ? expression : Expression.And(expression);
            }

            #endregion
        }
    }
}