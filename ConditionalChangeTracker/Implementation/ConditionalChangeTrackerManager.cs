using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public class ConditionalChangeTrackerFactory<EntityType> : AChangeTrackerHandler<EntityType>, IConditionalChangeTrackerFactory<EntityType> where EntityType : class, new()
    {
        protected List<IConditionalChangeTracker<EntityType>> ConditionalChangeTrackers = new List<IConditionalChangeTracker<EntityType>>();

        public ConditionalChangeTrackerFactory(IChangeTracker<EntityType> changeTracker) : base(changeTracker)
        {
        }

        public override void Process(IEnumerable<EntityChange<EntityType>> entities)
        {
            ConditionalChangeTrackers.ForEach(tracker => tracker.Process(entities));
        }

        public IConditionalChangeTracker<EntityType> Where(Expression<Func<EntityType, bool>> expression)
        {
            var tracker = ConditionalChangeTrackers.FirstOrDefault(t => LambdaCompare.Eq(t.Expression, expression));
            if (tracker == null)
            {
                if(expression == null)
                {
                    expression = e => true;
                }
                tracker = new ConditionalChangeTracker<EntityType>(expression);
                ConditionalChangeTrackers.Add(tracker);
            }
            return tracker;
        }
    }

    public class MapChangeTrackerFactory<EntityType> : AChangeTrackerHandler<EntityType>, IMapChangeTrackerFactory<EntityType> where EntityType : class, new()
    {
        protected Dictionary<Type, List<IEntityChangeProcessor<EntityType>>> MapChangeTrackers = new Dictionary<Type, List<IEntityChangeProcessor<EntityType>>>();

        protected List<IEntityChangeProcessor<EntityType>> GetMapChangeTrackersAsObject<ToEntityType>()
        {
            var t = typeof(ToEntityType);
            if (!MapChangeTrackers.ContainsKey(t))
            {
                return MapChangeTrackers[t] = new List<IEntityChangeProcessor<EntityType>>();
            }
            return MapChangeTrackers[t];
        }

        protected List<IMapChangeTracker<EntityType, ToEntityType>> GetMapChangeTrackers<ToEntityType>() where ToEntityType : class, new()
        {
            return GetMapChangeTrackersAsObject<ToEntityType>().Select(m => m as IMapChangeTracker<EntityType, ToEntityType>).ToList();
        }

        protected void AddMapChangeTracker<ToEntityType>(IMapChangeTracker<EntityType, ToEntityType> mapChangeTracker) where ToEntityType : class, new()
        {
            GetMapChangeTrackersAsObject<ToEntityType>().Add(mapChangeTracker);
        }

        //protected List<IMapChangeTracker<EntityType, ToEntityType>> MapChangeTrackers = new List<IMapChangeTracker<EntityType, ToEntityType>>();

        protected DbContext _db;

        public MapChangeTrackerFactory(IChangeTracker<EntityType> changeTracker, DbContext db) : base(changeTracker)
        {
            _db = db;
        }

        public override void Process(IEnumerable<EntityChange<EntityType>> entities)
        {
            MapChangeTrackers.Select(pair => pair.Value).ToList().ForEach(list => list.ForEach(tracker => tracker.Process(entities)));
        }

        public IMapChangeTracker<EntityType, ToEntityType> Select<ToEntityType>(Expression <Func<EntityType, ToEntityType>> expression) where ToEntityType : class, new()
        {
            var tracker = GetMapChangeTrackers<ToEntityType>().FirstOrDefault(t => LambdaCompare.Eq(t.Mapping, expression));
            if (tracker == null)
            {
                tracker = new MapChangeTracker<EntityType, ToEntityType>(expression, _db);
                GetMapChangeTrackersAsObject<ToEntityType>().Add(tracker);
            }
            return tracker;
        }
    }


    public class MapChangeTracker<EntityType, ToEntityType> : IMapChangeTracker<EntityType, ToEntityType>, IChangeTracker<ToEntityType>
        where ToEntityType : class, new()
        where EntityType : class, new()
    {
        DbContext _db;

        public MapChangeTracker(Expression<Func<EntityType, ToEntityType>> expression, DbContext db)
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
