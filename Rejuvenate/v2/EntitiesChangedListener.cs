using Rejuvenate.v2.EntityChangePublishing;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2
{
    public class EntitiesChangedListener<EntityType> where EntityType : class, new()
    {
        public EntitiesChangedListener(Expression<Func<EntityType, bool>> expression, EntitiesChangedHandler<EntityType> handler, Guid id)
        {
            Expression = expression;
            Handler = handler;
            Id = id;
        }

        public Guid Id;

        public Expression<Func<EntityType, bool>> Expression;

        public EntitiesChangedHandler<EntityType> Handler;

        public void Receive(IEnumerable<EntityChangeMessage<EntityType>> messages)
        {
            IEnumerable<EntityChangeMessage<EntityType>> all;
            if (Expression == null)
            {
                all = messages;
            }
            else
            {
                var expressionDlg = Expression.Compile();

                var added = messages.Where(message => message.State == EntityState.Added && expressionDlg(message.Current));
                var added2 = messages.Where(message => message.State == EntityState.Modified && expressionDlg(message.Current) && !expressionDlg(message.Last))
                    .Select(message => new EntityChangeMessage<EntityType>(EntityState.Added, null, message.Current));
                var deleted = messages.Where(message => message.State == EntityState.Deleted && expressionDlg(message.Last));
                var deleted2 = messages.Where(message => message.State == EntityState.Modified && !expressionDlg(message.Current) && expressionDlg(message.Last))
                    .Select(message => new EntityChangeMessage<EntityType>(EntityState.Deleted, message.Last, null));
                var modified = messages.Where(message => message.State == EntityState.Modified && expressionDlg(message.Current) && expressionDlg(message.Last));

                all = added.Concat(added2).Concat(deleted).Concat(deleted2).Concat(modified);
            }

            if (all.Count() > 0)
                Handler(all, this);
        }
    }
}
