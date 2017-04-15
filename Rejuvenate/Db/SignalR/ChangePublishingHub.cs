using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;



namespace Rejuvenate.Db.SignalR
{
    /*
                // todo DI gebruiken 
                // send data to frontend
                DbContext.Items.Add(new Item() { Name = "poepie" });
                DbContext.SaveChanges();
                //context.Items...
                //Clients.Caller.updateData(context.Items);
                */

    // This is where commands from the webclient (player ui) are executed
    public class ChangePublishingHub : Hub
    {
        public static List<ISignalRSubscriber> Subscribers = new List<ISignalRSubscriber>();

        public void Subscribe(List<int> publisherIds)
        {
            var client = new SignalRSubscriber(Context.ConnectionId);
            client.PublisherIds.AddRange(publisherIds);
            Subscribers.Add(client);
        }
    }

    public class SignalRHubPublisher<HubType> where HubType : IHub
    {
        public void Publish<EntityType>(Type type, int publisherId, EntityState state, IEnumerable<EntityType> entries)
        {
            var clients = ChangePublishingHub.Subscribers.Where(client => client.PublisherIds.Contains(publisherId));
            var context = GlobalHost.ConnectionManager.GetHubContext<HubType>();
            foreach (var client in clients)
            {
                switch (state)
                {
                    case EntityState.Added:
                        context.Clients.Client(client.ConnectionId).itemsAdded(type, publisherId, entries);
                        break;
                    case EntityState.Deleted:
                        context.Clients.Client(client.ConnectionId).itemsRemoved(type, publisherId, entries);
                        break;
                    case EntityState.Modified:
                        context.Clients.Client(client.ConnectionId).itemsUpdated(type, publisherId, entries);
                        break;
                }
            }
        }
    }
}