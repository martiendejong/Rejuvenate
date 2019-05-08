/*using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public static class ChangePublishingQueryableExtension
    {
        public static IPublisher<EntityType, HubType> Publisher<HubType, EntityType>(this IChangePublishingQueryable<EntityType> queryable) where EntityType : class, new() where HubType : IHub
        {
            var q = queryable;
            var fn = q.Condition;
            var pp = ChangePublishingSignalRHub.Publishers.Select(p => p.Value).OfType<Publisher<EntityType, HubType>>();
            var playerPublisher = pp.FirstOrDefault(p => LambdaCompare.Eq(p.Condition, fn));
            if (playerPublisher == null)
            {
                playerPublisher = new Publisher<EntityType, HubType> { Condition = fn };
                ChangePublishingSignalRHub.Publishers.Add(playerPublisher.Id, playerPublisher);
                q.EntitiesChanged += playerPublisher.Publish;
            }
            return playerPublisher;
        }
    }
}*/
