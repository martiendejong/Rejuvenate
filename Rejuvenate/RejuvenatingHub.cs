using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace Rejuvenate
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
    public class RejuvenatingHub : Hub
    {
        public static List<IRejuvenatingClient> RejuvenatingClients = new List<IRejuvenatingClient>();

        public void RegisterRejuvenatingClient(List<int> rejuvenatorIds)
        {
            var client = new RejuvenatingClient(Context.ConnectionId);
            client.RejuvenatorIds.AddRange(rejuvenatorIds);
            RejuvenatingClients.Add(client);
        }
    }

    public class SignalRHubRejuvenator<HubType> where HubType : IHub
    {
        public void Rejuvenate<EntityType>(Type type, int rejuvenatorId, EntityState state, IEnumerable<EntityType> entries)
        {
            var clients = RejuvenatingHub.RejuvenatingClients.Where(client => client.RejuvenatorIds.Contains(rejuvenatorId));
            var context = GlobalHost.ConnectionManager.GetHubContext<HubType>();
            foreach (var client in clients)
            {
                switch (state)
                {
                    case EntityState.Added:
                        context.Clients.Client(client.ConnectionId).itemsAdded(type, rejuvenatorId, entries);
                        break;
                    case EntityState.Deleted:
                        context.Clients.Client(client.ConnectionId).itemsRemoved(type, rejuvenatorId, entries);
                        break;
                    case EntityState.Modified:
                        context.Clients.Client(client.ConnectionId).itemsUpdated(type, rejuvenatorId, entries);
                        break;
                }
            }
        }
    }
}