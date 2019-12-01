using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Rejuvenate
{
    public class EntityChangeMappingProcessorFactory<EntityType> : AChangesProcessor<EntityType>, IEntityChangeMappingProcessorFactory<EntityType> where EntityType : class, new()
    {
        protected Dictionary<Type, List<IChangesProcessor<EntityType>>> MapChangeTrackers = new Dictionary<Type, List<IChangesProcessor<EntityType>>>();

        protected List<IChangesProcessor<EntityType>> GetMapChangeTrackersAsObject<ToEntityType>()
        {
            var t = typeof(ToEntityType);
            if (!MapChangeTrackers.ContainsKey(t))
            {
                return MapChangeTrackers[t] = new List<IChangesProcessor<EntityType>>();
            }
            return MapChangeTrackers[t];
        }

        protected List<IEntityChangeMappingProcessor<EntityType, ToEntityType>> GetMapChangeTrackers<ToEntityType>() where ToEntityType : class, new()
        {
            return GetMapChangeTrackersAsObject<ToEntityType>().Select(m => m as IEntityChangeMappingProcessor<EntityType, ToEntityType>).ToList();
        }

        protected void AddMapChangeTracker<ToEntityType>(IEntityChangeMappingProcessor<EntityType, ToEntityType> mapChangeTracker) where ToEntityType : class, new()
        {
            GetMapChangeTrackersAsObject<ToEntityType>().Add(mapChangeTracker);
        }

        //protected List<IMapChangeTracker<EntityType, ToEntityType>> MapChangeTrackers = new List<IMapChangeTracker<EntityType, ToEntityType>>();

        protected DbContext _db;

        public EntityChangeMappingProcessorFactory(IChangesPublisher<EntityType> changeTracker, DbContext db) : base(changeTracker)
        {
            _db = db;
        }

        public override void Process(IEnumerable<EntityChange<EntityType>> entities)
        {
            MapChangeTrackers.Select(pair => pair.Value).ToList().ForEach(list => list.ForEach(tracker => tracker.Process(entities)));
        }

        public IEntityChangeMappingProcessor<EntityType, ToEntityType> Select<ToEntityType>(Expression <Func<EntityType, ToEntityType>> expression) where ToEntityType : class, new()
        {
            var tracker = GetMapChangeTrackers<ToEntityType>().FirstOrDefault(t => LambdaCompare.Eq(t.Mapping, expression));
            if (tracker == null)
            {
                tracker = new EntityChangeMappingProcessor<EntityType, ToEntityType>(expression, _db);
                GetMapChangeTrackersAsObject<ToEntityType>().Add(tracker);
            }
            return tracker;
        }
    }
}
