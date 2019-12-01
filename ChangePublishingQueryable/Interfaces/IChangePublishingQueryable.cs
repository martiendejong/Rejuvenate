using Rejuvenate;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public interface IHubPublisher<EntityType> where EntityType : class, new()
    {
        IPublisher<EntityType, HubType> Publisher<HubType>() where HubType : IHub;
    }

    public interface IChangePublishingQueryable<EntityType> : IQueryable<EntityType>, IHubPublisher<EntityType> where EntityType : class, new()
    {
        event EntitiesChangedHandler<EntityType> EntitiesChanged;

        Expression<Func<EntityType, bool>> Filter { get; }

        IChangePublishingQueryable<EntityType> Where(Expression<Func<EntityType, bool>> expression);

        IChangePublishingQueryable<ToEntityType> Select<ToEntityType>(Expression<Func<EntityType, ToEntityType>> expression) where ToEntityType : class, new();

        IChangePublishingValue<EntityType, int> Sum(Expression<Func<EntityType, int>> selector);
    }
}
