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
        public static List<RejuvenatingClient> RejuvenatingClients = new List<RejuvenatingClient>();

        public void RegisterRejuvenatingClient(params int[] rejuvenatorId)
        {
            RejuvenatingClient client = new RejuvenatingClient(Context.ConnectionId);
            RejuvenatingClients.Add(client);
        }
    }
}