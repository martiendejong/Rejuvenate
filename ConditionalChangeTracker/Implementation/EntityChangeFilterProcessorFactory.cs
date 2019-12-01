using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public class EntityChangeFilterProcessorFactory<EntityType> : AChangesProcessor<EntityType>, IEntityChangeFilterProcessorFactory<EntityType> where EntityType : class, new()
    {
        protected List<IChangesFilterProcessor<EntityType>> Processors = new List<IChangesFilterProcessor<EntityType>>();

        public EntityChangeFilterProcessorFactory(IChangesPublisher<EntityType> entityChangeBus) : base(entityChangeBus)
        {
        }

        public override void Process(IEnumerable<EntityChange<EntityType>> entities)
        {
            Processors.ForEach(processor => processor.Process(entities));
        }

        public IChangesFilterProcessor<EntityType> Where(Expression<Func<EntityType, bool>> filter)
        {
            var processor = Processors.FirstOrDefault(_processor => LambdaCompare.Eq(_processor.FilterExpression, filter));
            if (processor == null)
            {
                if(filter == null)
                {
                    filter = entityType => true;
                }
                processor = new EntityChangeFilterProcessor<EntityType>(filter);
                Processors.Add(processor);
            }
            return processor;
        }
    }
}
