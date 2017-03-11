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
        public static ISignalRClients SignalRClients = new SignalRClients();
  
        public void RegisterAwarenessClient(params int[] pollerIds)
        {
            AwarenessClient client = new AwarenessClient(Context.ConnectionId);
            SignalRClients.AwarenessClients.Add(client);
        }
    }
}