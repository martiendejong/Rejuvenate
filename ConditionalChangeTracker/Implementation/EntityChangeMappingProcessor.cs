using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Rejuvenate
{
    public class EntityChangeMappingProcessor<EntityType, ToEntityType> : IEntityChangeMappingProcessor<EntityType, ToEntityType>, IChangesPublisher<ToEntityType>
        where ToEntityType : class, new()
        where EntityType : class, new()
    {
        readonly DbContext _db;

        public EntityChangeMappingProcessor(Expression<Func<EntityType, ToEntityType>> expression, DbContext db)
        {
            _db = db;
            _expression = expression;
        }

        protected Expression<Func<EntityType, ToEntityType>> _expression;

        public event EntitiesChangedHandler<ToEntityType> EntitiesChanged;

        public Expression<Func<EntityType, ToEntityType>> Mapping => _expression;

        public void Process(IEnumerable<EntityChange<EntityType>> entities)
        {
            var currents = entities.Select(change => _db.Entry(change.Current).Entity).AsQueryable();
            var result = currents.Select(Mapping).Select(cur => new EntityChange<ToEntityType>(
                EntityState.Modified,
                cur,
                cur));
            EntitiesChanged?.Invoke(result);
        }
    }
}
