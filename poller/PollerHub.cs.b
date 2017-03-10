using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace poller
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
    public class PollerHub : Hub
    {
        public static ISignalRClients Clients = new SignalRClients();
        
        // todo place in separate class
        public static void PublishItems(Type type, int pollerId, EntityState state, IEnumerable<Item> entries)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            switch(state)
            {
                case EntityState.Added:
                    context.Clients.All.itemsAdded(type, pollerId, entries);
                    break;
                case EntityState.Deleted:
                    context.Clients.All.itemsRemoved(type, pollerId, entries);
                    break;
                case EntityState.Modified:
                    context.Clients.All.itemsUpdated(type, pollerId, entries);
                    break;
            }
        }
  
        public void RegisterAwarenessClient(params int[] pollerIds)
        {
            AwarenessClient client = new AwarenessClient(Context.ConnectionId);
            Clients.AwarenessClients.Add(client);
        }
    }
}