using Rejuvenate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Rejuvenate
{
    public class EntityChangeFilterProcessor<EntityType> : IChangesFilterProcessor<EntityType> where EntityType : class, new()
    {
        public Expression<Func<EntityType, bool>> FilterExpression { get; private set; }

        private Func<EntityType, bool> FilterFn;

        public event EntitiesChangedHandler<EntityType> EntitiesChanged;

        public EntityChangeFilterProcessor(Expression<Func<EntityType, bool>> filterExpression)
        {
            FilterExpression = filterExpression;
            FilterFn = FilterExpression.Compile();
        }

        private List<EntityChange<EntityType>> Filter(IEnumerable<EntityChange<EntityType>> entities)
        {
            var entitiesQueryable = entities.AsQueryable();
            var filteredEntities = new List<EntityChange<EntityType>>();

            filteredEntities.AddRange(
                entitiesQueryable.Where(change =>
                    change.Current != null
                    && change.Previous != null
                    && FilterFn.Invoke(change.Current)
                    && FilterFn.Invoke(change.Previous)
                )
            );
            filteredEntities.AddRange(
                entitiesQueryable.Where(change =>
                    change.Current != null
                    && FilterFn.Invoke(change.Current)
                    && (
                        change.Previous == null
                        || !FilterFn.Invoke(change.Previous)
                    )
                )
                .Select(change => new EntityChange<EntityType>(EntityState.Added, null, change.Current))
            );
            filteredEntities.AddRange(
                entitiesQueryable.Where(change =>
                    change.State != EntityState.Added
                    && (
                        change.Current == null
                        | !FilterFn.Invoke(change.Current)
                    )
                    && change.Previous != null
                    && FilterFn.Invoke(change.Previous)
                )
                .Select(change => new EntityChange<EntityType>(EntityState.Deleted, change.Previous, null))
            );

            return filteredEntities;
        }

        public void Process(IEnumerable<EntityChange<EntityType>> entities)
        {
            if (EntitiesChanged != null)
            {
                var filteredEntities = Filter(entities);
                if (filteredEntities.Count > 0)
                {
                    EntitiesChanged.Invoke(filteredEntities);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
