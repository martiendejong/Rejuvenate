﻿using Rejuvenate.v2.EntityChangePublishing;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate.v2
{
    public class LinkedEntityChangedHandler<ChildEntityType, EntityType/*, PrimaryKeyType*/> where ChildEntityType : class, new() where EntityType : class, new()
    { 
        public ChangePublishingDbContext DbContext;

       /* public Func<PrimaryKeyType, EntityType> GetEntityByPrimaryKey;

        public Func<ChildEntityType, PrimaryKeyType> PathToForeignKey;

        

        public Expression<Func<EntityType, bool>> Expression;*/

        public EntitiesChangedHandler<EntityType> Handler;

        public EntitiesChangedListener<EntityType> Channel;




        public Expression<Func<ChildEntityType, EntityType>> Reference;
        



        public void Receive(IEnumerable<EntityChangeMessage<ChildEntityType>> childMessages)
        {
            var modified = childMessages.Where(message => message.Current != null && message.Last != null);
            foreach (var msg in modified)
            {
                DbContext.Entry(msg.Current).Reference(Reference).Load();
                //DbContext.Entry(msg.Last).Reference(Reference).Load();
            }
            var modifiedCurrentParents = modified.Select(msg => msg.Current).Select(Reference.Compile());
            //var modifiedLastParents = modified.Select(msg => msg.Last).Select(Reference.Compile());
            var messages = modifiedCurrentParents.Select(entity => new EntityChangeMessage<EntityType>(EntityState.Modified, null, entity));
            Handler(messages, Channel);

            /*var entitiesToWhichTheChildIsAdded = childMessages.Where(message => message.Current != null && message.Last == null).Select(message => message.Current);
            foreach (var ent in entitiesToWhichTheChildIsAdded)
            {
                DbContext.Entry(ent).Reference(Reference).Load();
            }
            var parentEntities = entitiesToWhichTheChildIsAdded.Select(Reference.Compile());

            //DbContext.Entry(myPerson).Reference(p => p.Address).Load();

            var keys = childMessages.Where(message => message.Current != null && message.Last == null).Select(message => message.Current).Select(PathToForeignKey);
            var addedEntities = keys.Select(GetEntityByPrimaryKey).AsQueryable();
            if(Expression != null)
            {
                addedEntities = addedEntities.Where(Expression);
            }
            var messages = addedEntities.Select(entity => new EntityChangeMessage<EntityType>(EntityState.Modified, null, entity));
            Handler(messages, Channel);

            keys = childMessages.Select(message => message.Last).Where(childEntity => childEntity != null).Select(PathToForeignKey);
            var removedEntities = keys.Select(GetEntityByPrimaryKey).AsQueryable();
            if (Expression != null)
            {
                removedEntities = removedEntities.Where(Expression);
            }
            messages = removedEntities.Select(entity => new EntityChangeMessage<EntityType>(EntityState.Modified, null, entity));
            Handler(messages, Channel);*/
        }

        /*public Expression<Func<EntityType, bool>> Expression;

        public Expression<Func<LinkedEntityType, EntityType>> select;

        public int publisherId;

        public Func<IQueryable<EntityIdType>, IQueryable<EntityType>> resolveEntityById;

        public Expression<Func<LinkedEntityType, EntityIdType>> foreignKeySelect;

        public void ChangedHandler(Type type, int rejuvenatorId, EntityState state, IEnumerable<KeyValuePair<LinkedEntityType, LinkedEntityType>> subEntityPairs)
        {
            var signalRHubPublisher = new SignalRHubPublisher<HubType>();

            // rejuvenate the entities that are linked to the subentity
            var entities = subEntityPairs.Select(entity => entity.Key).AsQueryable().Select(select);
            entities = (Expression == null ? entities : entities.Where(Expression)).Distinct();
            signalRHubPublisher.Publish(type, publisherId, EntityState.Modified, entities);

            // rejuvenate the entities that are unlinked from the subentity
            var originalSubEntities = subEntityPairs.Where(pair => pair.Value != null).Select(pair => pair.Value);
            var originalEntityIds = originalSubEntities.AsQueryable().Where(entity => entity != null).Select(foreignKeySelect).Distinct();


            var updatedOrgEntities = resolveEntityById(originalEntityIds);
            updatedOrgEntities = (Expression == null ? updatedOrgEntities : updatedOrgEntities.Where(Expression));
            signalRHubPublisher.Publish(type, publisherId, EntityState.Modified, updatedOrgEntities);
        }*/
    }
}