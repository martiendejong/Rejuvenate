using ChangePublishingDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ChangePublishingDbContext
{
    public class ConditionalChangeTracker<EntityType> : IConditionalChangeTracker<EntityType> where EntityType : class, new()
    {
        public Expression<Func<EntityType, bool>> Expression { get; private set; }

        private Func<EntityType, bool> Fn;

        public event EntitiesChangedHandler<EntityType> EntitiesChanged;

        public ConditionalChangeTracker(Expression<Func<EntityType, bool>> expression)
        {
            Expression = expression;
            Fn = Expression.Compile();
        }

        public void Process(IEnumerable<EntityChange<EntityType>> entities)
        {
            var q = entities.AsQueryable();
            var newEntities = new List<EntityChange<EntityType>>();

            newEntities.AddRange(
                q.Where(change => 
                    change.Current != null 
                    && change.Previous != null 
                    && Fn.Invoke(change.Current) 
                    && Fn.Invoke(change.Previous)
                )
            );
            newEntities.AddRange(
                q.Where(change => 
                    change.Current != null 
                    && Fn.Invoke(change.Current) 
                    && (
                        change.Previous == null 
                        || !Fn.Invoke(change.Previous)
                    )
                )
                .Select(change => new EntityChange<EntityType>(EntityState.Added, null,change.Current))
            );
            newEntities.AddRange(
                q.Where(change => 
                    change.State != EntityState.Added
                    && (
                        change.Current == null 
                        | !Fn.Invoke(change.Current)
                    )
                    && change.Previous != null
                    && Fn.Invoke(change.Previous)
                )
                .Select(change => new EntityChange<EntityType>(EntityState.Deleted, change.Previous, null))
            );

            if (newEntities.Count > 0 && EntitiesChanged != null)
            {
                EntitiesChanged.Invoke(newEntities);
            }
        }
    }
}
