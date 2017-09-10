using EFExtensions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2.Internal
{
    public class EntitiesChangedListeners
    {
        protected List<object> Listeners = new List<object>();

        protected ChangePublishingDbContext DbContext;

        public EntitiesChangedListeners(ChangePublishingDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public EntitiesChangedListener<EntityType> GetListener<EntityType>(Expression<Func<EntityType, bool>> expression, EntitiesChangedHandler<EntityType> handler) where EntityType : class, new()
        {
            var entityTypeListeners = Listeners.OfType<EntitiesChangedListeners<EntityType>>().FirstOrDefault();
            if(entityTypeListeners == null)
            {
                entityTypeListeners = new EntitiesChangedListeners<EntityType>(DbContext);
                Listeners.Add(entityTypeListeners);
            }
            return entityTypeListeners.GetListener(expression, handler);
        }
    }

    public class EntitiesChangedListeners<EntityType> where EntityType : class, new()
    {
        public EntitiesChangedListeners(ChangePublishingDbContext dbContext)
        {
            DbContext = dbContext;
        }

        // Subscribe an event handler
        public EntitiesChangedListener<EntityType> GetListener(Expression<Func<EntityType, bool>> expression, EntitiesChangedHandler<EntityType> handler)
        {
            var listener = findListener(expression, handler);
            if (listener == null)
            {
                listener = createListener(expression, handler);
            }
            return listener;
        }

        protected ChangePublishingDbContext DbContext;

        protected List<EntitiesChangedListener<EntityType>> Listeners = new List<EntitiesChangedListener<EntityType>>();

        protected EntitiesChangedListener<EntityType> findListener(Expression<Func<EntityType, bool>> expression, EntitiesChangedHandler<EntityType> listener)
        {
            foreach (var exprHandler in Listeners)
            {
                if (LambdaCompare.Eq(exprHandler.Expression, expression) && exprHandler.Handler == listener)
                {
                    return exprHandler;
                }
            }
            return null;
        }

        protected EntitiesChangedListener<EntityType> createListener(Expression<Func<EntityType, bool>> expression, EntitiesChangedHandler<EntityType> handler)
        {
            EntitiesChangedListener<EntityType> listener;
            Guid guid = generateListenerId();

            listener = new EntitiesChangedListener<EntityType>(expression, handler, guid);
            Listeners.Add(listener);
            DbContext.Hub.Subscribe<EntityType>(listener.Receive);
            return listener;
        }

        protected Guid generateListenerId()
        {
            Guid guid;
            do
            {
                guid = Guid.NewGuid();
            }
            while (Listeners.Any(c => c.Id == guid));
            return guid;
        }
    }
}
